using System.Text.Json;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public class EventConverter : JsonConverter<Event>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Event).IsAssignableFrom(typeToConvert);
        }

        public override Event? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
            if (Enum.TryParse(jsonDoc.RootElement.GetProperty("Type").GetString() ?? throw new JsonException("Could not find property 'Type' on event object"), out EventType type)) {
                return type switch {
                    EventType.Action => jsonDoc.RootElement.Deserialize<ActionEvent>(options),
                    EventType.Fork => jsonDoc.RootElement.Deserialize<ForkEvent>(options),
                    EventType.Join => jsonDoc.RootElement.Deserialize<JoinEvent>(options),
                    EventType.Subflow => jsonDoc.RootElement.Deserialize<SubflowEvent>(options),
                    EventType.Switch => jsonDoc.RootElement.Deserialize<SwitchEvent>(options),
                    _ => throw new JsonException($"Type '{type}' does not match a known event type"),
                };
            }
            else {
                throw new JsonException("Invalid type on event object");
            }
        }

        public override void Write(Utf8JsonWriter writer, Event eventBase, JsonSerializerOptions options)
        {
            if (eventBase.Type == EventType.Action) {
                JsonSerializer.Serialize(writer, (ActionEvent)eventBase, options);
            }
            else if (eventBase.Type == EventType.Fork) {
                JsonSerializer.Serialize(writer, (ForkEvent)eventBase, options);
            }
            else if (eventBase.Type == EventType.Join) {
                JsonSerializer.Serialize(writer, (JoinEvent)eventBase, options);
            }
            else if (eventBase.Type == EventType.Subflow) {
                JsonSerializer.Serialize(writer, (SubflowEvent)eventBase, options);
            }
            else if (eventBase.Type == EventType.Switch) {
                JsonSerializer.Serialize(writer, (SwitchEvent)eventBase, options);
            }
            else {
                throw new JsonException("Invalid event type");
            }
        }
    }
}
