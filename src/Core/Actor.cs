using EvflLibrary.Common;
using EvflLibrary.Parsers;
using Microsoft.Win32;

namespace EvflLibrary.Core
{
    public class Actor : IEvflDataBlock
    {
        internal Action? insertActionsPtr = null;
        internal Action? insertQueriesPtr = null;
        internal Action? insertParamsPtr = null;

        public string Name { get; set; }
        public string SecondaryName { get; set; }
        public string ArgumentName { get; set; }
        public short EntryPointIndex { get; set; }

        /// <summary>
        /// <para>(?) Cut number? This is set to 1 for flowcharts. Timeline actors sometimes use a different value here.</para>
        /// <para>In BotW, this value is passed as the @MA actor parameter. https://zeldamods.org/wiki/BFEVFL#Actor</para>
        /// </summary>
        public byte CutNumber { get; set; }

        public List<string> Actions { get; set; }
        public List<string> Queries { get; set; }
        public Container? Parameters { get; set; }

        public Actor() { }
        public Actor(EvflReader reader)
        {
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            Name = reader.ReadStringPtr();
            SecondaryName = reader.ReadStringPtr();
            ArgumentName = reader.ReadStringPtr();

            long actionsOffset = reader.ReadInt64();
            long queriesOffset = reader.ReadInt64();
            Parameters = reader.ReadObjectPtr<Container>(() => new(reader));
            ushort actionCount = reader.ReadUInt16();
            ushort queryCount = reader.ReadUInt16();

            EntryPointIndex = reader.ReadInt16();
            CutNumber = reader.ReadByte();

            // Padding
            reader.BaseStream.Position += 1;

            Actions = new(reader.ReadObjectsPtr(new string[actionCount], reader.ReadStringPtr, actionsOffset));
            Queries = new(reader.ReadObjectsPtr(new string[queryCount], reader.ReadStringPtr, queriesOffset));

            return this;
        }

        public void Write(EvflWriter writer)
        {
            WriteHeader(writer);
            writer.ReserveBlockWriter("ActorArrayDataBlock", () => {
                WriteData(writer);
            });
        }

        public void WriteHeader(EvflWriter writer)
        {
            writer.WriteStringPtr(Name);
            writer.WriteStringPtr(SecondaryName);
            writer.WriteStringPtr(ArgumentName);

            CheckAction(ref insertActionsPtr, Actions.Count, register: true);
            CheckAction(ref insertQueriesPtr, Queries.Count, register: true);
            CheckAction(ref insertParamsPtr, Parameters?.Count ?? 0, register: false);

            writer.Write((ushort)Actions.Count);
            writer.Write((ushort)Queries.Count);
            writer.Write(EntryPointIndex);
            writer.Write(CutNumber);
            writer.Write(new byte());

            void CheckAction(ref Action? action, int count, bool register)
            {
                if (action == null) {
                    action = writer.ReservePtrIf(count > 0, register: register);
                }
                else {
                    action();
                }
            }
        }

        public void WriteData(EvflWriter writer)
        {
            if ((Parameters?.Count ?? 0) > 0) {
                writer.Align(8);

                if (insertParamsPtr == null) {
                    long pos = writer.BaseStream.Position;
                    insertParamsPtr = () => {
                        writer.RegisterPtr();
                        writer.Write(pos);
                    };
                }
                else {
                    insertParamsPtr();
                }

                Parameters!.Write(writer);
            }

            if (Actions.Count > 0) {
                writer.Align(8);

                if (insertActionsPtr == null) {
                    long pos = writer.BaseStream.Position;
                    insertActionsPtr = () => {
                        writer.RegisterPtr();
                        writer.Write(pos);
                    };
                }
                else {
                    insertActionsPtr();
                }

                for (int i = 0; i < Actions.Count; i++) {
                    writer.WriteStringPtr(Actions[i]);
                }
            }

            if (Queries.Count > 0) {
                writer.Align(8);

                if (insertQueriesPtr == null) {
                    long pos = writer.BaseStream.Position;
                    insertQueriesPtr = () => {
                        writer.RegisterPtr();
                        writer.Write(pos);
                    };
                }
                else {
                    insertQueriesPtr();
                }

                for (int i = 0; i < Queries.Count; i++) {
                    writer.WriteStringPtr(Queries[i]);
                }
            }
        }
    }
}
