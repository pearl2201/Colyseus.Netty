using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server
{
    // Colyseus protocol codes range between 0~100
    public enum Protocol
    {
        // Room-related (10~19)
        JOIN_ROOM = 10,
        ERROR = 11,
        LEAVE_ROOM = 12,
        ROOM_DATA = 13,
        ROOM_STATE = 14,
        ROOM_STATE_PATCH = 15,
        ROOM_DATA_SCHEMA = 16, // used to send schema instances via room.send()

        // WebSocket close codes (https://github.com/Luka967/websocket-close-codes)
        WS_CLOSE_NORMAL = 1000,

        // WebSocket error codes
        WS_CLOSE_CONSENTED = 4000,
        WS_CLOSE_WITH_ERROR = 4002,
        WS_SERVER_DISCONNECT = 4201,
        WS_TOO_MANY_CLIENTS = 4202,
    }

    public enum ErrorCode
    {
        // MatchMaking Error Codes
        MATCHMAKE_NO_HANDLER = 4210,
        MATCHMAKE_INVALID_CRITERIA = 4211,
        MATCHMAKE_INVALID_ROOM_ID = 4212,
        MATCHMAKE_UNHANDLED = 4213, // generic exception during onCreate/onJoin
        MATCHMAKE_EXPIRED = 4214, // generic exception during onCreate/onJoin

        AUTH_FAILED = 4215,
        APPLICATION_ERROR = 4216,
    }

    // Inter-process communication protocol
    public enum IpcProtocol
    {
        SUCCESS = 0,
        ERROR = 1,
        TIMEOUT = 2,
    }
}
