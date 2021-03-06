﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WRL.dto;
using WRL.utils;

namespace WRL.strategy
{
    //调用DLL实现策略
    class CallDLLStrategy : IQuoteStrategy
    {
        public void invokeLocalApp(dto.InputDTO inputDTO, dto.OutputDTO outputDTO, string serviceInstallPath)
        {

            string localAppPath = Path.Combine(serviceInstallPath, inputDTO.path);
            //判断本地程序是否存在
            if (!File.Exists(localAppPath))
            {
                outputDTO.code = ResultCode.Erroe;
                outputDTO.msg = "【第三方程序集不存在】";

                return;
            }

            List<ParamDTo> paramDTOLIst = inputDTO.param;
            Type[] parameterTypes = new Type[paramDTOLIst.Count]; // 实参类型
            object[] parameters = new object[paramDTOLIst.Count];//实参
            Type typeReturn = TypeUtil.getTypeByString(inputDTO.returnType); //返回类型
            DynamicLoadDLL.ModePass[] themode = new DynamicLoadDLL.ModePass[paramDTOLIst.Count]; //传递方式

            for (int i = 0; i < paramDTOLIst.Count; i++)
            {
                parameterTypes[i] = TypeUtil.getTypeByString(paramDTOLIst[i].type);
                parameters[i] = TypeUtil.getObjByType(paramDTOLIst[i].type, paramDTOLIst[i].value);
                themode[i] = (DynamicLoadDLL.ModePass)int.Parse(paramDTOLIst[i].mode);
            }

            Directory.SetCurrentDirectory(Path.GetDirectoryName(localAppPath));
            DynamicLoadDLL dld = new DynamicLoadDLL();
            dld.LoadDll(localAppPath);
            dld.LoadFun(inputDTO.method);
            object result = dld.Invoke(parameters, parameterTypes, themode, typeReturn);

            for (int i = 0; i < themode.Length; i++)
            {
                if (themode[i] != DynamicLoadDLL.ModePass.ByValue)
                {
                    outputDTO.returns.values.Add(parameters[i].ToString());
                }
            }
            outputDTO.code = ResultCode.Success;
            outputDTO.returns.result = result;
        }
    }
}