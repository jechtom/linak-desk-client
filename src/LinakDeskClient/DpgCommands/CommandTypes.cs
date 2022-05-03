using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient.DpgCommands
{
    public enum CommandTypes
    {
        PRODUCT_INFO = 8,
        GET_SETUP = 10,
        GET_CAPABILITIES = 128,
        DESK_OFFSET = 129,
        USER_ID = 134,
        GET_SET_REMINDER_TIME = 135,
        REMINDER_SETTING = 136,
        GET_SET_MEMORY_POSITION_1 = 137,
        GET_SET_MEMORY_POSITION_2 = 138,
        GET_SET_MEMORY_POSITION_3 = 139,
        GET_SET_MEMORY_POSITION_4 = 140, // not supported on IKEA IDASEN
        GET_LOG_ENTRY = 144
    }
}
