﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WRL.dto;

namespace WRL.strategy
{
    //策略模式接口
    interface IQuoteStrategy
    {
        void invokeLocalApp(InputDTO inputDTO, OutputDTO outputDTO, string serviceInstallPath);

    }
}
