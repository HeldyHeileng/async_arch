using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proto.Tools;
public static class SerializerExtension
{
    public static string ToProtobufString(this object model)
    {
        using var memString = new MemoryStream();
        Serializer.Serialize(memString, model);
        return Convert.ToBase64String(memString.GetBuffer(), 0, (int)memString.Length);
    }

    public static T? FromProtobufString<T>(this string protobuf) where T : class
    {
        try
        {
            var byteAfter64 = Convert.FromBase64String(protobuf);
            using var mem = new MemoryStream(byteAfter64);
            return ProtoBuf.Serializer.Deserialize<T>(mem);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string GetHeader(this Headers headers, string headerKey)
    {
        string result = "";
        foreach (var header in headers)
        {
            if (header.Key != headerKey)
            {
                continue;
            }

            result = Encoding.UTF8.GetString(headers[0].GetValueBytes());
        }

        return result;
    }
}

[ProtoContract]
public class DateTimeWrapper
{
    private DateTime _date;

    [ProtoMember(1)]
    public DateTime Date
    {
        get { return _date; }
        set { _date = new DateTime(value.Ticks, DateTimeKind.Utc); }
    }
}