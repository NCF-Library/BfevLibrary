using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvflLibrary.Extensions;

namespace EvflLibrary.Core
{
    public class Actor
    {
        public string Name;
        public string SecondaryName;
        public string ArgumentName;
        public long ActionsOffset;
        public long QueriesOffset;
        public long ParamsOffset;
        public ushort ActionCount;
        public ushort QueryCount;
        public ushort EntryPointIndex; // Entry point index for associated entry point (0xffff if none)
        public byte CutNumber; // (?) Cut number? This is set to 1 for flowcharts. Timeline actors sometimes use a different value here. In BotW, this value is passed as the @MA actor parameter. https://zeldamods.org/wiki/BFEVFL#Actor
        
        public string[] Actions;
        public string[] Queries;
        public Container Parameters;

        public Actor(BinaryReader reader)
        {
            Name = reader.ReadStringPtr();
            SecondaryName = reader.ReadStringPtr();
            ArgumentName = reader.ReadStringPtr();
            ActionsOffset = reader.ReadInt64();
            QueriesOffset = reader.ReadInt64();
            ParamsOffset = reader.ReadInt64();
            ActionCount = reader.ReadUInt16();
            QueryCount = reader.ReadUInt16();
            EntryPointIndex = reader.ReadUInt16();
            CutNumber = reader.ReadByte();
            reader.BaseStream.Position += 1;

            Actions = new string[ActionCount];
            reader.TemporarySeek(ActionsOffset, SeekOrigin.Begin, () => {
                for (int i = 0; i < ActionCount; i++) {
                    Actions[i] = reader.ReadStringPtr();
                }
            });

            Queries = new string[QueryCount];
            reader.TemporarySeek(QueriesOffset, SeekOrigin.Begin, () => {
                for (int i = 0; i < QueryCount; i++) {
                    Queries[i] = reader.ReadStringPtr();
                }
            });

            Parameters = reader.TemporarySeek<Container>(ParamsOffset, SeekOrigin.Begin, () => {
                return new(reader);
            });
        }
    }
}
