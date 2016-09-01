﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HyperMock.Core
{
    internal class VisitList
    {
        internal VisitList()
        {
            RecordedVisits = new List<Visit>();
        }

        internal List<Visit> RecordedVisits { get; }

        internal Visit Record(string name, object[] args)
        {
            var visit = RecordedVisits.FirstOrDefault(v => v.Name == name && IsMatchFor(v.Args, args));

            if (visit != null)
            {
                visit.VisitCount++;
            }
            else
            {
                visit = new Visit(name, args);
                RecordedVisits.Add(visit);
            }

            return visit;
        }

        internal Visit FindBy(LambdaExpression expression, CallType callType, object[] values = null)
        {
            switch (callType)
            {
                case CallType.Method:
                case CallType.Function:
                    return FindMethodVisit(expression);
                case CallType.GetProperty:
                    return FindGetPropertyVisit(expression, values);
                case CallType.SetProperty:
                    return FindSetPropertyVisit(expression, values);
            }

            return null;
        }
        
        private Visit FindMethodVisit(LambdaExpression expression)
        {
            var body = expression.Body as MethodCallExpression;

            if (body == null) return null;

            var matchingVisitationsByName = RecordedVisits.Where(v => v.Name == body.Method.Name).ToList();

            if (!matchingVisitationsByName.Any()) return null;

            var parameterList = new ParameterList();
            var parameters = parameterList.BuildFrom(body, expression);

            return matchingVisitationsByName.FirstOrDefault(v => parameterList.IsMatchFor(parameters, v.Args));
        }

        private Visit FindGetPropertyVisit(LambdaExpression expression, object[] values = null)
        {
            var body = expression.Body as MemberExpression;

            if (body == null) return null;

            var propInfo = (PropertyInfo)body.Member;
            var getMethodInfo = propInfo.GetMethod;

            if (getMethodInfo == null) return null;

            if (values != null && values.Length > 0)
                return RecordedVisits.FirstOrDefault(v => v.Name == getMethodInfo.Name && IsMatchFor(values, v.Args));

            return RecordedVisits.FirstOrDefault(v => v.Name == getMethodInfo.Name);
        }

        private Visit FindSetPropertyVisit(LambdaExpression expression, object[] values = null)
        {
            var body = expression.Body as MemberExpression;

            if (body == null) return null;

            var propInfo = (PropertyInfo)body.Member;
            var setMethodInfo = propInfo.SetMethod;

            if (setMethodInfo == null) return null;

            if (values != null && values.Length > 0)
                return RecordedVisits.FirstOrDefault(v => v.Name == setMethodInfo.Name && IsMatchFor(values, v.Args));

            return RecordedVisits.FirstOrDefault(v => v.Name == setMethodInfo.Name);
        }

        private static bool IsMatchFor(object[] args, object[] otherArgs)
        {
            if (args == null && otherArgs == null) return true;
            if (args == null || otherArgs == null) return false;
            if (args.Length != otherArgs.Length) return false;

            for (var i = 0; i < args.Length; i++)
            {
                if (!Equals(args[i], otherArgs[i])) return false;
            }

            return true;
        }
    }
}
