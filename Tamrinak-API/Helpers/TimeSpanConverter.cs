﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tamrinak_API.Helpers
{
	public class TimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return TimeSpan.Parse(reader.GetString()!);
		}

		public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
		}
	}
}
