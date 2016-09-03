﻿using System;
using System.Linq;
using System.Linq.Expressions;
#if WINDOWS_UWP
using System.Reflection;
#endif

namespace HyperMock.Matchers
{
    internal class ParameterMatcherFactory
    {
        internal ParameterMatcher Create(LambdaExpression expression)
        {
            var methodCall = expression.Body as MethodCallExpression;

            // Find the parameter matcher attribute (if exists)
            var paramMatcherAttr = methodCall?.Method.GetCustomAttributes(
                    typeof(ParameterMatcherAttribute), false).FirstOrDefault() as ParameterMatcherAttribute;

            // If exists then create the matcher else return the exact matcher
            return paramMatcherAttr != null 
                ? CreateFromExpression(paramMatcherAttr.ParameterMatcherType, expression) 
                : new ExactParameterMatcher();
        }

        private ParameterMatcher CreateFromExpression(Type paramMatcherType, LambdaExpression expression)
        {
            var methodCall = (MethodCallExpression)expression.Body;

            var matcher = (ParameterMatcher)Activator.CreateInstance(paramMatcherType);
            matcher.CallContext = methodCall;
            return matcher;
        }
    }
}