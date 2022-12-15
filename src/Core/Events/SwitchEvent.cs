using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class SwitchEvent : Event, IEvflDataBlock
    {
        public record SwitchCase(uint Value, ushort EventIndex);

        public ushort ActorIndex { get; set; }
        public ushort ActorQueryIndex { get; set; }
        public Container? Parameters { get; set; }
        public List<SwitchCase> SwitchCases { get; set; }

        public SwitchEvent(EvflReader reader, Event baseEvent) : base(baseEvent)
        {
            ushort switchCaseCount = reader.ReadUInt16();
            ActorIndex = reader.ReadUInt16();
            ActorQueryIndex = reader.ReadUInt16();
            Parameters = reader.ReadObjectPtr<Container>(() => new(reader));
            SwitchCases = reader.ReadObjectsPtr(new SwitchCase[switchCaseCount], () => {
                SwitchCase switchCase = new(reader.ReadUInt32(), reader.ReadUInt16());
                reader.ReadUInt16();
                reader.Align(8);
                return switchCase;
            }).ToList();
            reader.BaseStream.Position += 8; // Unused pointer
        }

        public new void Write(EvflWriter writer)
        {
            base.Write(writer);
            writer.Write((ushort)SwitchCases.Count);
            writer.Write(ActorIndex);
            writer.Write(ActorQueryIndex);
            Action insertContainerPtr = writer.ReservePtrIf(Parameters != null);
            Action insertSwitchCasesPtr = writer.ReservePtrIf(SwitchCases.Count > 0, register: true);
            writer.Write(0L);
            writer.ReserveBlockWriter("EventArrayDataBlock", () => {
                if (SwitchCases.Count > 0) {
                    writer.Align(8);
                    insertSwitchCasesPtr();
                    for (int i = 0; i < SwitchCases.Count; i++) {
                        SwitchCase switchCase = SwitchCases[i];
                        writer.Write(switchCase.Value);
                        writer.Write(switchCase.EventIndex);
                        writer.Write((ushort)0); // Padding
                        writer.Align(8);
                    }
                }

                insertContainerPtr();
                Parameters?.Write(writer);
            });
        }
    }
}
