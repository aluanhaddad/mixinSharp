﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class MethodSymbolReader : SemanticTypeReaderBase
    {
        private readonly IMethodList _methods;

        public MethodSymbolReader(IMethodList methodList)
        {
            _methods = methodList;
        }

        protected override void ReadSymbol(IMethodSymbol methodSymbol)
        {
            // skip all property accessors and ctors
            if (methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                var isOverrideFromObject = 
                    methodSymbol.IsOverride &&
                    methodSymbol.OverriddenMethod
                    ?.ContainingType.SpecialType == SpecialType.System_Object;
                var method = new Method(methodSymbol.Name, methodSymbol.ReturnType, isOverrideFromObject);
                var parameterReader = new ParameterSymbolReader(method);
                parameterReader.VisitSymbol(methodSymbol);
                _methods.AddMethod(method);
            }
        }
    }    
}