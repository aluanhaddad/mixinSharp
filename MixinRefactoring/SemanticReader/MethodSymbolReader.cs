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
            // we don't need to know about static members
            // because they won't be delegated from child to mixin
            if (methodSymbol.IsStatic)
                return;

            // skip all property accessors and ctors
            if (methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                var isOverrideFromObject = 
                    methodSymbol.IsOverride &&
                    methodSymbol.OverriddenMethod
                    ?.ContainingType.SpecialType == SpecialType.System_Object;
                var method = new Method(
                    methodSymbol.Name, 
                    methodSymbol.ReturnType, 
                    isOverrideFromObject);

                method.IsAbstract = methodSymbol.IsAbstract;
                method.IsOverride = methodSymbol.IsOverride;
                
                method.Documentation = new DocumentationComment(methodSymbol.GetDocumentationCommentXml());

                var parameterReader = new ParameterSymbolReader(method);
                parameterReader.VisitSymbol(methodSymbol);
                _methods.AddMethod(method);
            }
        }
    }    
}
