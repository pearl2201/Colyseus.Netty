using Colyseus.Server;

public class ColyseusMessage {
    public int Cmd { get; set; }
    public int Version { get; set; }

    public int Len { get; set; }

    public byte[] RawData {get;set;}
}

public class JMessage {

    public JMetadata Meta {get;set;}

    public object Data {get;set;}

    public JError[] Errors {get;set;}

}

public class JMetadata {
    public bool Success {get;set;}

    public string Summary {get;set;}
}

public class JError {
    public int Code {get;set;}

    public string Message {get;set;}

    public string Field {get;set;}
}

public class JStatus {

}