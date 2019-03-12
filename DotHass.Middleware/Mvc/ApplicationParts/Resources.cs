﻿namespace DotHass.Middleware.Mvc.ApplicationParts
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.AspNetCore.Mvc.Core.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// The argument '{0}' is invalid. Media types which match all types or match all subtypes are not supported.
        /// </summary>
        internal static string MatchAllContentTypeIsNotAllowed
        {
            get => GetString("MatchAllContentTypeIsNotAllowed");
        }

        /// <summary>
        /// The argument '{0}' is invalid. Media types which match all types or match all subtypes are not supported.
        /// </summary>
        internal static string FormatMatchAllContentTypeIsNotAllowed(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("MatchAllContentTypeIsNotAllowed"), p0);

        /// <summary>
        /// The content-type '{0}' added in the '{1}' property is invalid. Media types which match all types or match all subtypes are not supported.
        /// </summary>
        internal static string ObjectResult_MatchAllContentType
        {
            get => GetString("ObjectResult_MatchAllContentType");
        }

        /// <summary>
        /// The content-type '{0}' added in the '{1}' property is invalid. Media types which match all types or match all subtypes are not supported.
        /// </summary>
        internal static string FormatObjectResult_MatchAllContentType(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ObjectResult_MatchAllContentType"), p0, p1);

        /// <summary>
        /// The method '{0}' on type '{1}' returned an instance of '{2}'. Make sure to call Unwrap on the returned value to avoid unobserved faulted Task.
        /// </summary>
        internal static string ActionExecutor_WrappedTaskInstance
        {
            get => GetString("ActionExecutor_WrappedTaskInstance");
        }

        /// <summary>
        /// The method '{0}' on type '{1}' returned an instance of '{2}'. Make sure to call Unwrap on the returned value to avoid unobserved faulted Task.
        /// </summary>
        internal static string FormatActionExecutor_WrappedTaskInstance(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("ActionExecutor_WrappedTaskInstance"), p0, p1, p2);

        /// <summary>
        /// The method '{0}' on type '{1}' returned a Task instance even though it is not an asynchronous method.
        /// </summary>
        internal static string ActionExecutor_UnexpectedTaskInstance
        {
            get => GetString("ActionExecutor_UnexpectedTaskInstance");
        }

        /// <summary>
        /// The method '{0}' on type '{1}' returned a Task instance even though it is not an asynchronous method.
        /// </summary>
        internal static string FormatActionExecutor_UnexpectedTaskInstance(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ActionExecutor_UnexpectedTaskInstance"), p0, p1);

        /// <summary>
        /// An action invoker could not be created for action '{0}'.
        /// </summary>
        internal static string ActionInvokerFactory_CouldNotCreateInvoker
        {
            get => GetString("ActionInvokerFactory_CouldNotCreateInvoker");
        }

        /// <summary>
        /// An action invoker could not be created for action '{0}'.
        /// </summary>
        internal static string FormatActionInvokerFactory_CouldNotCreateInvoker(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ActionInvokerFactory_CouldNotCreateInvoker"), p0);

        /// <summary>
        /// The action descriptor must be of type '{0}'.
        /// </summary>
        internal static string ActionDescriptorMustBeBasedOnControllerAction
        {
            get => GetString("ActionDescriptorMustBeBasedOnControllerAction");
        }

        /// <summary>
        /// The action descriptor must be of type '{0}'.
        /// </summary>
        internal static string FormatActionDescriptorMustBeBasedOnControllerAction(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ActionDescriptorMustBeBasedOnControllerAction"), p0);

        /// <summary>
        /// Value cannot be null or empty.
        /// </summary>
        internal static string ArgumentCannotBeNullOrEmpty
        {
            get => GetString("ArgumentCannotBeNullOrEmpty");
        }

        /// <summary>
        /// Value cannot be null or empty.
        /// </summary>
        internal static string FormatArgumentCannotBeNullOrEmpty()
            => GetString("ArgumentCannotBeNullOrEmpty");

        /// <summary>
        /// The '{0}' property of '{1}' must not be null.
        /// </summary>
        internal static string PropertyOfTypeCannotBeNull
        {
            get => GetString("PropertyOfTypeCannotBeNull");
        }

        /// <summary>
        /// The '{0}' property of '{1}' must not be null.
        /// </summary>
        internal static string FormatPropertyOfTypeCannotBeNull(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("PropertyOfTypeCannotBeNull"), p0, p1);

        /// <summary>
        /// The '{0}' method of type '{1}' cannot return a null value.
        /// </summary>
        internal static string TypeMethodMustReturnNotNullValue
        {
            get => GetString("TypeMethodMustReturnNotNullValue");
        }

        /// <summary>
        /// The '{0}' method of type '{1}' cannot return a null value.
        /// </summary>
        internal static string FormatTypeMethodMustReturnNotNullValue(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("TypeMethodMustReturnNotNullValue"), p0, p1);

        /// <summary>
        /// The value '{0}' is invalid.
        /// </summary>
        internal static string ModelBinding_NullValueNotValid
        {
            get => GetString("ModelBinding_NullValueNotValid");
        }

        /// <summary>
        /// The value '{0}' is invalid.
        /// </summary>
        internal static string FormatModelBinding_NullValueNotValid(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelBinding_NullValueNotValid"), p0);

        /// <summary>
        /// The passed expression of expression node type '{0}' is invalid. Only simple member access expressions for model properties are supported.
        /// </summary>
        internal static string Invalid_IncludePropertyExpression
        {
            get => GetString("Invalid_IncludePropertyExpression");
        }

        /// <summary>
        /// The passed expression of expression node type '{0}' is invalid. Only simple member access expressions for model properties are supported.
        /// </summary>
        internal static string FormatInvalid_IncludePropertyExpression(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("Invalid_IncludePropertyExpression"), p0);

        /// <summary>
        /// No route matches the supplied values.
        /// </summary>
        internal static string NoRoutesMatched
        {
            get => GetString("NoRoutesMatched");
        }

        /// <summary>
        /// No route matches the supplied values.
        /// </summary>
        internal static string FormatNoRoutesMatched()
            => GetString("NoRoutesMatched");

        /// <summary>
        /// If an {0} provides a result value by setting the {1} property of {2} to a non-null value, then it cannot call the next filter by invoking {3}.
        /// </summary>
        internal static string AsyncActionFilter_InvalidShortCircuit
        {
            get => GetString("AsyncActionFilter_InvalidShortCircuit");
        }

        /// <summary>
        /// If an {0} provides a result value by setting the {1} property of {2} to a non-null value, then it cannot call the next filter by invoking {3}.
        /// </summary>
        internal static string FormatAsyncActionFilter_InvalidShortCircuit(object p0, object p1, object p2, object p3)
            => string.Format(CultureInfo.CurrentCulture, GetString("AsyncActionFilter_InvalidShortCircuit"), p0, p1, p2, p3);

        /// <summary>
        /// If an {0} cancels execution by setting the {1} property of {2} to 'true', then it cannot call the next filter by invoking {3}.
        /// </summary>
        internal static string AsyncResultFilter_InvalidShortCircuit
        {
            get => GetString("AsyncResultFilter_InvalidShortCircuit");
        }

        /// <summary>
        /// If an {0} cancels execution by setting the {1} property of {2} to 'true', then it cannot call the next filter by invoking {3}.
        /// </summary>
        internal static string FormatAsyncResultFilter_InvalidShortCircuit(object p0, object p1, object p2, object p3)
            => string.Format(CultureInfo.CurrentCulture, GetString("AsyncResultFilter_InvalidShortCircuit"), p0, p1, p2, p3);

        /// <summary>
        /// The type provided to '{0}' must implement '{1}'.
        /// </summary>
        internal static string FilterFactoryAttribute_TypeMustImplementIFilter
        {
            get => GetString("FilterFactoryAttribute_TypeMustImplementIFilter");
        }

        /// <summary>
        /// The type provided to '{0}' must implement '{1}'.
        /// </summary>
        internal static string FormatFilterFactoryAttribute_TypeMustImplementIFilter(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("FilterFactoryAttribute_TypeMustImplementIFilter"), p0, p1);

        /// <summary>
        /// Cannot return null from an action method with a return type of '{0}'.
        /// </summary>
        internal static string ActionResult_ActionReturnValueCannotBeNull
        {
            get => GetString("ActionResult_ActionReturnValueCannotBeNull");
        }

        /// <summary>
        /// Cannot return null from an action method with a return type of '{0}'.
        /// </summary>
        internal static string FormatActionResult_ActionReturnValueCannotBeNull(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ActionResult_ActionReturnValueCannotBeNull"), p0);

        /// <summary>
        /// The type '{0}' must derive from '{1}'.
        /// </summary>
        internal static string TypeMustDeriveFromType
        {
            get => GetString("TypeMustDeriveFromType");
        }

        /// <summary>
        /// The type '{0}' must derive from '{1}'.
        /// </summary>
        internal static string FormatTypeMustDeriveFromType(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("TypeMustDeriveFromType"), p0, p1);

        /// <summary>
        /// No encoding found for input formatter '{0}'. There must be at least one supported encoding registered in order for the formatter to read content.
        /// </summary>
        internal static string InputFormatterNoEncoding
        {
            get => GetString("InputFormatterNoEncoding");
        }

        /// <summary>
        /// No encoding found for input formatter '{0}'. There must be at least one supported encoding registered in order for the formatter to read content.
        /// </summary>
        internal static string FormatInputFormatterNoEncoding(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InputFormatterNoEncoding"), p0);

        /// <summary>
        /// Unsupported content type '{0}'.
        /// </summary>
        internal static string UnsupportedContentType
        {
            get => GetString("UnsupportedContentType");
        }

        /// <summary>
        /// Unsupported content type '{0}'.
        /// </summary>
        internal static string FormatUnsupportedContentType(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("UnsupportedContentType"), p0);

        /// <summary>
        /// No supported media type registered for output formatter '{0}'. There must be at least one supported media type registered in order for the output formatter to write content.
        /// </summary>
        internal static string OutputFormatterNoMediaType
        {
            get => GetString("OutputFormatterNoMediaType");
        }

        /// <summary>
        /// No supported media type registered for output formatter '{0}'. There must be at least one supported media type registered in order for the output formatter to write content.
        /// </summary>
        internal static string FormatOutputFormatterNoMediaType(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("OutputFormatterNoMediaType"), p0);

        /// <summary>
        /// The following errors occurred with attribute routing information:{0}{0}{1}
        /// </summary>
        internal static string AttributeRoute_AggregateErrorMessage
        {
            get => GetString("AttributeRoute_AggregateErrorMessage");
        }

        /// <summary>
        /// The following errors occurred with attribute routing information:{0}{0}{1}
        /// </summary>
        internal static string FormatAttributeRoute_AggregateErrorMessage(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_AggregateErrorMessage"), p0, p1);

        /// <summary>
        /// The attribute route '{0}' cannot contain a parameter named '{{{1}}}'. Use '[{1}]' in the route template to insert the value '{2}'.
        /// </summary>
        internal static string AttributeRoute_CannotContainParameter
        {
            get => GetString("AttributeRoute_CannotContainParameter");
        }

        /// <summary>
        /// The attribute route '{0}' cannot contain a parameter named '{{{1}}}'. Use '[{1}]' in the route template to insert the value '{2}'.
        /// </summary>
        internal static string FormatAttributeRoute_CannotContainParameter(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_CannotContainParameter"), p0, p1, p2);

        /// <summary>
        /// For action: '{0}'{1}Error: {2}
        /// </summary>
        internal static string AttributeRoute_IndividualErrorMessage
        {
            get => GetString("AttributeRoute_IndividualErrorMessage");
        }

        /// <summary>
        /// For action: '{0}'{1}Error: {2}
        /// </summary>
        internal static string FormatAttributeRoute_IndividualErrorMessage(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_IndividualErrorMessage"), p0, p1, p2);

        /// <summary>
        /// An empty replacement token ('[]') is not allowed.
        /// </summary>
        internal static string AttributeRoute_TokenReplacement_EmptyTokenNotAllowed
        {
            get => GetString("AttributeRoute_TokenReplacement_EmptyTokenNotAllowed");
        }

        /// <summary>
        /// An empty replacement token ('[]') is not allowed.
        /// </summary>
        internal static string FormatAttributeRoute_TokenReplacement_EmptyTokenNotAllowed()
            => GetString("AttributeRoute_TokenReplacement_EmptyTokenNotAllowed");

        /// <summary>
        /// Token delimiters ('[', ']') are imbalanced.
        /// </summary>
        internal static string AttributeRoute_TokenReplacement_ImbalancedSquareBrackets
        {
            get => GetString("AttributeRoute_TokenReplacement_ImbalancedSquareBrackets");
        }

        /// <summary>
        /// Token delimiters ('[', ']') are imbalanced.
        /// </summary>
        internal static string FormatAttributeRoute_TokenReplacement_ImbalancedSquareBrackets()
            => GetString("AttributeRoute_TokenReplacement_ImbalancedSquareBrackets");

        /// <summary>
        /// The route template '{0}' has invalid syntax. {1}
        /// </summary>
        internal static string AttributeRoute_TokenReplacement_InvalidSyntax
        {
            get => GetString("AttributeRoute_TokenReplacement_InvalidSyntax");
        }

        /// <summary>
        /// The route template '{0}' has invalid syntax. {1}
        /// </summary>
        internal static string FormatAttributeRoute_TokenReplacement_InvalidSyntax(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_TokenReplacement_InvalidSyntax"), p0, p1);

        /// <summary>
        /// While processing template '{0}', a replacement value for the token '{1}' could not be found. Available tokens: '{2}'. To use a '[' or ']' as a literal string in a route or within a constraint, use '[[' or ']]' instead.
        /// </summary>
        internal static string AttributeRoute_TokenReplacement_ReplacementValueNotFound
        {
            get => GetString("AttributeRoute_TokenReplacement_ReplacementValueNotFound");
        }

        /// <summary>
        /// While processing template '{0}', a replacement value for the token '{1}' could not be found. Available tokens: '{2}'. To use a '[' or ']' as a literal string in a route or within a constraint, use '[[' or ']]' instead.
        /// </summary>
        internal static string FormatAttributeRoute_TokenReplacement_ReplacementValueNotFound(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_TokenReplacement_ReplacementValueNotFound"), p0, p1, p2);

        /// <summary>
        /// A replacement token is not closed.
        /// </summary>
        internal static string AttributeRoute_TokenReplacement_UnclosedToken
        {
            get => GetString("AttributeRoute_TokenReplacement_UnclosedToken");
        }

        /// <summary>
        /// A replacement token is not closed.
        /// </summary>
        internal static string FormatAttributeRoute_TokenReplacement_UnclosedToken()
            => GetString("AttributeRoute_TokenReplacement_UnclosedToken");

        /// <summary>
        /// An unescaped '[' token is not allowed inside of a replacement token. Use '[[' to escape.
        /// </summary>
        internal static string AttributeRoute_TokenReplacement_UnescapedBraceInToken
        {
            get => GetString("AttributeRoute_TokenReplacement_UnescapedBraceInToken");
        }

        /// <summary>
        /// An unescaped '[' token is not allowed inside of a replacement token. Use '[[' to escape.
        /// </summary>
        internal static string FormatAttributeRoute_TokenReplacement_UnescapedBraceInToken()
            => GetString("AttributeRoute_TokenReplacement_UnescapedBraceInToken");

        /// <summary>
        /// Unable to find the required services. Please add all the required services by calling '{0}.{1}' inside the call to '{2}' in the application startup code.
        /// </summary>
        internal static string UnableToFindServices
        {
            get => GetString("UnableToFindServices");
        }

        /// <summary>
        /// Unable to find the required services. Please add all the required services by calling '{0}.{1}' inside the call to '{2}' in the application startup code.
        /// </summary>
        internal static string FormatUnableToFindServices(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("UnableToFindServices"), p0, p1, p2);

        /// <summary>
        /// Action: '{0}' - Template: '{1}'
        /// </summary>
        internal static string AttributeRoute_DuplicateNames_Item
        {
            get => GetString("AttributeRoute_DuplicateNames_Item");
        }

        /// <summary>
        /// Action: '{0}' - Template: '{1}'
        /// </summary>
        internal static string FormatAttributeRoute_DuplicateNames_Item(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_DuplicateNames_Item"), p0, p1);

        /// <summary>
        /// Attribute routes with the same name '{0}' must have the same template:{1}{2}
        /// </summary>
        internal static string AttributeRoute_DuplicateNames
        {
            get => GetString("AttributeRoute_DuplicateNames");
        }

        /// <summary>
        /// Attribute routes with the same name '{0}' must have the same template:{1}{2}
        /// </summary>
        internal static string FormatAttributeRoute_DuplicateNames(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_DuplicateNames"), p0, p1, p2);

        /// <summary>
        /// Error {0}:{1}{2}
        /// </summary>
        internal static string AttributeRoute_AggregateErrorMessage_ErrorNumber
        {
            get => GetString("AttributeRoute_AggregateErrorMessage_ErrorNumber");
        }

        /// <summary>
        /// Error {0}:{1}{2}
        /// </summary>
        internal static string FormatAttributeRoute_AggregateErrorMessage_ErrorNumber(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_AggregateErrorMessage_ErrorNumber"), p0, p1, p2);

        /// <summary>
        /// A method '{0}' must not define attribute routed actions and non attribute routed actions at the same time:{1}{2}{1}{1}Use 'AcceptVerbsAttribute' to create a single route that allows multiple HTTP verbs and defines a route, or set a route template in all attributes that constrain HTTP verbs.
        /// </summary>
        internal static string AttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod
        {
            get => GetString("AttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod");
        }

        /// <summary>
        /// A method '{0}' must not define attribute routed actions and non attribute routed actions at the same time:{1}{2}{1}{1}Use 'AcceptVerbsAttribute' to create a single route that allows multiple HTTP verbs and defines a route, or set a route template in all attributes that constrain HTTP verbs.
        /// </summary>
        internal static string FormatAttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod"), p0, p1, p2);

        /// <summary>
        /// Action: '{0}' - Route Template: '{1}' - HTTP Verbs: '{2}'
        /// </summary>
        internal static string AttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod_Item
        {
            get => GetString("AttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod_Item");
        }

        /// <summary>
        /// Action: '{0}' - Route Template: '{1}' - HTTP Verbs: '{2}'
        /// </summary>
        internal static string FormatAttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod_Item(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("AttributeRoute_MixedAttributeAndConventionallyRoutedActions_ForMethod_Item"), p0, p1, p2);

        /// <summary>
        /// (none)
        /// </summary>
        internal static string AttributeRoute_NullTemplateRepresentation
        {
            get => GetString("AttributeRoute_NullTemplateRepresentation");
        }

        /// <summary>
        /// (none)
        /// </summary>
        internal static string FormatAttributeRoute_NullTemplateRepresentation()
            => GetString("AttributeRoute_NullTemplateRepresentation");

        /// <summary>
        /// Multiple actions matched. The following actions matched route data and had all constraints satisfied:{0}{0}{1}
        /// </summary>
        internal static string DefaultActionSelector_AmbiguousActions
        {
            get => GetString("DefaultActionSelector_AmbiguousActions");
        }

        /// <summary>
        /// Multiple actions matched. The following actions matched route data and had all constraints satisfied:{0}{0}{1}
        /// </summary>
        internal static string FormatDefaultActionSelector_AmbiguousActions(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("DefaultActionSelector_AmbiguousActions"), p0, p1);

        /// <summary>
        /// Could not find file: {0}
        /// </summary>
        internal static string FileResult_InvalidPath
        {
            get => GetString("FileResult_InvalidPath");
        }

        /// <summary>
        /// Could not find file: {0}
        /// </summary>
        internal static string FormatFileResult_InvalidPath(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("FileResult_InvalidPath"), p0);

        /// <summary>
        /// The input was not valid.
        /// </summary>
        internal static string SerializableError_DefaultError
        {
            get => GetString("SerializableError_DefaultError");
        }

        /// <summary>
        /// The input was not valid.
        /// </summary>
        internal static string FormatSerializableError_DefaultError()
            => GetString("SerializableError_DefaultError");

        /// <summary>
        /// If an {0} provides a result value by setting the {1} property of {2} to a non-null value, then it cannot call the next filter by invoking {3}.
        /// </summary>
        internal static string AsyncResourceFilter_InvalidShortCircuit
        {
            get => GetString("AsyncResourceFilter_InvalidShortCircuit");
        }

        /// <summary>
        /// If an {0} provides a result value by setting the {1} property of {2} to a non-null value, then it cannot call the next filter by invoking {3}.
        /// </summary>
        internal static string FormatAsyncResourceFilter_InvalidShortCircuit(object p0, object p1, object p2, object p3)
            => string.Format(CultureInfo.CurrentCulture, GetString("AsyncResourceFilter_InvalidShortCircuit"), p0, p1, p2, p3);

        /// <summary>
        /// If the '{0}' property is not set to true, '{1}' property must be specified.
        /// </summary>
        internal static string ResponseCache_SpecifyDuration
        {
            get => GetString("ResponseCache_SpecifyDuration");
        }

        /// <summary>
        /// If the '{0}' property is not set to true, '{1}' property must be specified.
        /// </summary>
        internal static string FormatResponseCache_SpecifyDuration(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ResponseCache_SpecifyDuration"), p0, p1);

        /// <summary>
        /// The action '{0}' has ApiExplorer enabled, but is using conventional routing. Only actions which use attribute routing support ApiExplorer.
        /// </summary>
        internal static string ApiExplorer_UnsupportedAction
        {
            get => GetString("ApiExplorer_UnsupportedAction");
        }

        /// <summary>
        /// The action '{0}' has ApiExplorer enabled, but is using conventional routing. Only actions which use attribute routing support ApiExplorer.
        /// </summary>
        internal static string FormatApiExplorer_UnsupportedAction(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ApiExplorer_UnsupportedAction"), p0);

        /// <summary>
        /// The media type "{0}" is not valid. MediaTypes containing wildcards (*) are not allowed in formatter mappings.
        /// </summary>
        internal static string FormatterMappings_NotValidMediaType
        {
            get => GetString("FormatterMappings_NotValidMediaType");
        }

        /// <summary>
        /// The media type "{0}" is not valid. MediaTypes containing wildcards (*) are not allowed in formatter mappings.
        /// </summary>
        internal static string FormatFormatterMappings_NotValidMediaType(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("FormatterMappings_NotValidMediaType"), p0);

        /// <summary>
        /// The format provided is invalid '{0}'. A format must be a non-empty file-extension, optionally prefixed with a '.' character.
        /// </summary>
        internal static string Format_NotValid
        {
            get => GetString("Format_NotValid");
        }

        /// <summary>
        /// The format provided is invalid '{0}'. A format must be a non-empty file-extension, optionally prefixed with a '.' character.
        /// </summary>
        internal static string FormatFormat_NotValid(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("Format_NotValid"), p0);

        /// <summary>
        /// The '{0}' cache profile is not defined.
        /// </summary>
        internal static string CacheProfileNotFound
        {
            get => GetString("CacheProfileNotFound");
        }

        /// <summary>
        /// The '{0}' cache profile is not defined.
        /// </summary>
        internal static string FormatCacheProfileNotFound(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CacheProfileNotFound"), p0);

        /// <summary>
        /// The model's runtime type '{0}' is not assignable to the type '{1}'.
        /// </summary>
        internal static string ModelType_WrongType
        {
            get => GetString("ModelType_WrongType");
        }

        /// <summary>
        /// The model's runtime type '{0}' is not assignable to the type '{1}'.
        /// </summary>
        internal static string FormatModelType_WrongType(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelType_WrongType"), p0, p1);

        /// <summary>
        /// The type '{0}' cannot be activated by '{1}' because it is either a value type, an interface, an abstract class or an open generic type.
        /// </summary>
        internal static string ValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated
        {
            get => GetString("ValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated");
        }

        /// <summary>
        /// The type '{0}' cannot be activated by '{1}' because it is either a value type, an interface, an abstract class or an open generic type.
        /// </summary>
        internal static string FormatValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated"), p0, p1);

        /// <summary>
        /// The type '{0}' must implement '{1}' to be used as a model binder.
        /// </summary>
        internal static string BinderType_MustBeIModelBinder
        {
            get => GetString("BinderType_MustBeIModelBinder");
        }

        /// <summary>
        /// The type '{0}' must implement '{1}' to be used as a model binder.
        /// </summary>
        internal static string FormatBinderType_MustBeIModelBinder(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("BinderType_MustBeIModelBinder"), p0, p1);

        /// <summary>
        /// The provided binding source '{0}' is a composite. '{1}' requires that the source must represent a single type of input.
        /// </summary>
        internal static string BindingSource_CannotBeComposite
        {
            get => GetString("BindingSource_CannotBeComposite");
        }

        /// <summary>
        /// The provided binding source '{0}' is a composite. '{1}' requires that the source must represent a single type of input.
        /// </summary>
        internal static string FormatBindingSource_CannotBeComposite(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("BindingSource_CannotBeComposite"), p0, p1);

        /// <summary>
        /// The provided binding source '{0}' is a greedy data source. '{1}' does not support greedy data sources.
        /// </summary>
        internal static string BindingSource_CannotBeGreedy
        {
            get => GetString("BindingSource_CannotBeGreedy");
        }

        /// <summary>
        /// The provided binding source '{0}' is a greedy data source. '{1}' does not support greedy data sources.
        /// </summary>
        internal static string FormatBindingSource_CannotBeGreedy(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("BindingSource_CannotBeGreedy"), p0, p1);

        /// <summary>
        /// The property {0}.{1} could not be found.
        /// </summary>
        internal static string Common_PropertyNotFound
        {
            get => GetString("Common_PropertyNotFound");
        }

        /// <summary>
        /// The property {0}.{1} could not be found.
        /// </summary>
        internal static string FormatCommon_PropertyNotFound(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("Common_PropertyNotFound"), p0, p1);

        /// <summary>
        /// The key '{0}' is invalid JQuery syntax because it is missing a closing bracket.
        /// </summary>
        internal static string JQueryFormValueProviderFactory_MissingClosingBracket
        {
            get => GetString("JQueryFormValueProviderFactory_MissingClosingBracket");
        }

        /// <summary>
        /// The key '{0}' is invalid JQuery syntax because it is missing a closing bracket.
        /// </summary>
        internal static string FormatJQueryFormValueProviderFactory_MissingClosingBracket(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("JQueryFormValueProviderFactory_MissingClosingBracket"), p0);

        /// <summary>
        /// A value is required.
        /// </summary>
        internal static string KeyValuePair_BothKeyAndValueMustBePresent
        {
            get => GetString("KeyValuePair_BothKeyAndValueMustBePresent");
        }

        /// <summary>
        /// A value is required.
        /// </summary>
        internal static string FormatKeyValuePair_BothKeyAndValueMustBePresent()
            => GetString("KeyValuePair_BothKeyAndValueMustBePresent");

        /// <summary>
        /// The binding context has a null Model, but this binder requires a non-null model of type '{0}'.
        /// </summary>
        internal static string ModelBinderUtil_ModelCannotBeNull
        {
            get => GetString("ModelBinderUtil_ModelCannotBeNull");
        }

        /// <summary>
        /// The binding context has a null Model, but this binder requires a non-null model of type '{0}'.
        /// </summary>
        internal static string FormatModelBinderUtil_ModelCannotBeNull(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelBinderUtil_ModelCannotBeNull"), p0);

        /// <summary>
        /// The binding context has a Model of type '{0}', but this binder can only operate on models of type '{1}'.
        /// </summary>
        internal static string ModelBinderUtil_ModelInstanceIsWrong
        {
            get => GetString("ModelBinderUtil_ModelInstanceIsWrong");
        }

        /// <summary>
        /// The binding context has a Model of type '{0}', but this binder can only operate on models of type '{1}'.
        /// </summary>
        internal static string FormatModelBinderUtil_ModelInstanceIsWrong(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelBinderUtil_ModelInstanceIsWrong"), p0, p1);

        /// <summary>
        /// The binding context cannot have a null ModelMetadata.
        /// </summary>
        internal static string ModelBinderUtil_ModelMetadataCannotBeNull
        {
            get => GetString("ModelBinderUtil_ModelMetadataCannotBeNull");
        }

        /// <summary>
        /// The binding context cannot have a null ModelMetadata.
        /// </summary>
        internal static string FormatModelBinderUtil_ModelMetadataCannotBeNull()
            => GetString("ModelBinderUtil_ModelMetadataCannotBeNull");

        /// <summary>
        /// A value for the '{0}' property was not provided.
        /// </summary>
        internal static string ModelBinding_MissingBindRequiredMember
        {
            get => GetString("ModelBinding_MissingBindRequiredMember");
        }

        /// <summary>
        /// A value for the '{0}' property was not provided.
        /// </summary>
        internal static string FormatModelBinding_MissingBindRequiredMember(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelBinding_MissingBindRequiredMember"), p0);

        /// <summary>
        /// A non-empty request body is required.
        /// </summary>
        internal static string ModelBinding_MissingRequestBodyRequiredMember
        {
            get => GetString("ModelBinding_MissingRequestBodyRequiredMember");
        }

        /// <summary>
        /// A non-empty request body is required.
        /// </summary>
        internal static string FormatModelBinding_MissingRequestBodyRequiredMember()
            => GetString("ModelBinding_MissingRequestBodyRequiredMember");

        /// <summary>
        /// The parameter conversion from type '{0}' to type '{1}' failed because no type converter can convert between these types.
        /// </summary>
        internal static string ValueProviderResult_NoConverterExists
        {
            get => GetString("ValueProviderResult_NoConverterExists");
        }

        /// <summary>
        /// The parameter conversion from type '{0}' to type '{1}' failed because no type converter can convert between these types.
        /// </summary>
        internal static string FormatValueProviderResult_NoConverterExists(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ValueProviderResult_NoConverterExists"), p0, p1);

        /// <summary>
        /// Path '{0}' was not rooted.
        /// </summary>
        internal static string FileResult_PathNotRooted
        {
            get => GetString("FileResult_PathNotRooted");
        }

        /// <summary>
        /// Path '{0}' was not rooted.
        /// </summary>
        internal static string FormatFileResult_PathNotRooted(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("FileResult_PathNotRooted"), p0);

        /// <summary>
        /// The supplied URL is not local. A URL with an absolute path is considered local if it does not have a host/authority part. URLs using virtual paths ('~/') are also local.
        /// </summary>
        internal static string UrlNotLocal
        {
            get => GetString("UrlNotLocal");
        }

        /// <summary>
        /// The supplied URL is not local. A URL with an absolute path is considered local if it does not have a host/authority part. URLs using virtual paths ('~/') are also local.
        /// </summary>
        internal static string FormatUrlNotLocal()
            => GetString("UrlNotLocal");

        /// <summary>
        /// The argument '{0}' is invalid. Empty or null formats are not supported.
        /// </summary>
        internal static string FormatFormatterMappings_GetMediaTypeMappingForFormat_InvalidFormat
        {
            get => GetString("FormatFormatterMappings_GetMediaTypeMappingForFormat_InvalidFormat");
        }

        /// <summary>
        /// The argument '{0}' is invalid. Empty or null formats are not supported.
        /// </summary>
        internal static string FormatFormatFormatterMappings_GetMediaTypeMappingForFormat_InvalidFormat(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("FormatFormatterMappings_GetMediaTypeMappingForFormat_InvalidFormat"), p0);

        /// <summary>
        /// "Invalid values '{0}'."
        /// </summary>
        internal static string AcceptHeaderParser_ParseAcceptHeader_InvalidValues
        {
            get => GetString("AcceptHeaderParser_ParseAcceptHeader_InvalidValues");
        }

        /// <summary>
        /// "Invalid values '{0}'."
        /// </summary>
        internal static string FormatAcceptHeaderParser_ParseAcceptHeader_InvalidValues(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("AcceptHeaderParser_ParseAcceptHeader_InvalidValues"), p0);

        /// <summary>
        /// The value '{0}' is not valid for {1}.
        /// </summary>
        internal static string ModelState_AttemptedValueIsInvalid
        {
            get => GetString("ModelState_AttemptedValueIsInvalid");
        }

        /// <summary>
        /// The value '{0}' is not valid for {1}.
        /// </summary>
        internal static string FormatModelState_AttemptedValueIsInvalid(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelState_AttemptedValueIsInvalid"), p0, p1);

        /// <summary>
        /// The value '{0}' is not valid.
        /// </summary>
        internal static string ModelState_NonPropertyAttemptedValueIsInvalid
        {
            get => GetString("ModelState_NonPropertyAttemptedValueIsInvalid");
        }

        /// <summary>
        /// The value '{0}' is not valid.
        /// </summary>
        internal static string FormatModelState_NonPropertyAttemptedValueIsInvalid(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelState_NonPropertyAttemptedValueIsInvalid"), p0);

        /// <summary>
        /// The supplied value is invalid for {0}.
        /// </summary>
        internal static string ModelState_UnknownValueIsInvalid
        {
            get => GetString("ModelState_UnknownValueIsInvalid");
        }

        /// <summary>
        /// The supplied value is invalid for {0}.
        /// </summary>
        internal static string FormatModelState_UnknownValueIsInvalid(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelState_UnknownValueIsInvalid"), p0);

        /// <summary>
        /// The supplied value is invalid.
        /// </summary>
        internal static string ModelState_NonPropertyUnknownValueIsInvalid
        {
            get => GetString("ModelState_NonPropertyUnknownValueIsInvalid");
        }

        /// <summary>
        /// The supplied value is invalid.
        /// </summary>
        internal static string FormatModelState_NonPropertyUnknownValueIsInvalid()
            => GetString("ModelState_NonPropertyUnknownValueIsInvalid");

        /// <summary>
        /// The value '{0}' is invalid.
        /// </summary>
        internal static string HtmlGeneration_ValueIsInvalid
        {
            get => GetString("HtmlGeneration_ValueIsInvalid");
        }

        /// <summary>
        /// The value '{0}' is invalid.
        /// </summary>
        internal static string FormatHtmlGeneration_ValueIsInvalid(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("HtmlGeneration_ValueIsInvalid"), p0);

        /// <summary>
        /// The field {0} must be a number.
        /// </summary>
        internal static string HtmlGeneration_ValueMustBeNumber
        {
            get => GetString("HtmlGeneration_ValueMustBeNumber");
        }

        /// <summary>
        /// The field {0} must be a number.
        /// </summary>
        internal static string FormatHtmlGeneration_ValueMustBeNumber(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("HtmlGeneration_ValueMustBeNumber"), p0);

        /// <summary>
        /// The field must be a number.
        /// </summary>
        internal static string HtmlGeneration_NonPropertyValueMustBeNumber
        {
            get => GetString("HtmlGeneration_NonPropertyValueMustBeNumber");
        }

        /// <summary>
        /// The field must be a number.
        /// </summary>
        internal static string FormatHtmlGeneration_NonPropertyValueMustBeNumber()
            => GetString("HtmlGeneration_NonPropertyValueMustBeNumber");

        /// <summary>
        /// The list of '{0}' must not be empty. Add at least one supported encoding.
        /// </summary>
        internal static string TextInputFormatter_SupportedEncodingsMustNotBeEmpty
        {
            get => GetString("TextInputFormatter_SupportedEncodingsMustNotBeEmpty");
        }

        /// <summary>
        /// The list of '{0}' must not be empty. Add at least one supported encoding.
        /// </summary>
        internal static string FormatTextInputFormatter_SupportedEncodingsMustNotBeEmpty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("TextInputFormatter_SupportedEncodingsMustNotBeEmpty"), p0);

        /// <summary>
        /// The list of '{0}' must not be empty. Add at least one supported encoding.
        /// </summary>
        internal static string TextOutputFormatter_SupportedEncodingsMustNotBeEmpty
        {
            get => GetString("TextOutputFormatter_SupportedEncodingsMustNotBeEmpty");
        }

        /// <summary>
        /// The list of '{0}' must not be empty. Add at least one supported encoding.
        /// </summary>
        internal static string FormatTextOutputFormatter_SupportedEncodingsMustNotBeEmpty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("TextOutputFormatter_SupportedEncodingsMustNotBeEmpty"), p0);

        /// <summary>
        /// '{0}' is not supported by '{1}'. Use '{2}' instead.
        /// </summary>
        internal static string TextOutputFormatter_WriteResponseBodyAsyncNotSupported
        {
            get => GetString("TextOutputFormatter_WriteResponseBodyAsyncNotSupported");
        }

        /// <summary>
        /// '{0}' is not supported by '{1}'. Use '{2}' instead.
        /// </summary>
        internal static string FormatTextOutputFormatter_WriteResponseBodyAsyncNotSupported(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("TextOutputFormatter_WriteResponseBodyAsyncNotSupported"), p0, p1, p2);

        /// <summary>
        /// No media types found in '{0}.{1}'. Add at least one media type to the list of supported media types.
        /// </summary>
        internal static string Formatter_NoMediaTypes
        {
            get => GetString("Formatter_NoMediaTypes");
        }

        /// <summary>
        /// No media types found in '{0}.{1}'. Add at least one media type to the list of supported media types.
        /// </summary>
        internal static string FormatFormatter_NoMediaTypes(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("Formatter_NoMediaTypes"), p0, p1);

        /// <summary>
        /// Could not create a model binder for model object of type '{0}'.
        /// </summary>
        internal static string CouldNotCreateIModelBinder
        {
            get => GetString("CouldNotCreateIModelBinder");
        }

        /// <summary>
        /// Could not create a model binder for model object of type '{0}'.
        /// </summary>
        internal static string FormatCouldNotCreateIModelBinder(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CouldNotCreateIModelBinder"), p0);

        /// <summary>
        /// '{0}.{1}' must not be empty. At least one '{2}' is required to bind from the body.
        /// </summary>
        internal static string InputFormattersAreRequired
        {
            get => GetString("InputFormattersAreRequired");
        }

        /// <summary>
        /// '{0}.{1}' must not be empty. At least one '{2}' is required to bind from the body.
        /// </summary>
        internal static string FormatInputFormattersAreRequired(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("InputFormattersAreRequired"), p0, p1, p2);

        /// <summary>
        /// '{0}.{1}' must not be empty. At least one '{2}' is required to model bind.
        /// </summary>
        internal static string ModelBinderProvidersAreRequired
        {
            get => GetString("ModelBinderProvidersAreRequired");
        }

        /// <summary>
        /// '{0}.{1}' must not be empty. At least one '{2}' is required to model bind.
        /// </summary>
        internal static string FormatModelBinderProvidersAreRequired(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("ModelBinderProvidersAreRequired"), p0, p1, p2);

        /// <summary>
        /// '{0}.{1}' must not be empty. At least one '{2}' is required to format a response.
        /// </summary>
        internal static string OutputFormattersAreRequired
        {
            get => GetString("OutputFormattersAreRequired");
        }

        /// <summary>
        /// '{0}.{1}' must not be empty. At least one '{2}' is required to format a response.
        /// </summary>
        internal static string FormatOutputFormattersAreRequired(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("OutputFormattersAreRequired"), p0, p1, p2);

        /// <summary>
        /// Multiple overloads of method '{0}' are not supported.
        /// </summary>
        internal static string MiddewareFilter_ConfigureMethodOverload
        {
            get => GetString("MiddewareFilter_ConfigureMethodOverload");
        }

        /// <summary>
        /// Multiple overloads of method '{0}' are not supported.
        /// </summary>
        internal static string FormatMiddewareFilter_ConfigureMethodOverload(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddewareFilter_ConfigureMethodOverload"), p0);

        /// <summary>
        /// A public method named '{0}' could not be found in the '{1}' type.
        /// </summary>
        internal static string MiddewareFilter_NoConfigureMethod
        {
            get => GetString("MiddewareFilter_NoConfigureMethod");
        }

        /// <summary>
        /// A public method named '{0}' could not be found in the '{1}' type.
        /// </summary>
        internal static string FormatMiddewareFilter_NoConfigureMethod(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddewareFilter_NoConfigureMethod"), p0, p1);

        /// <summary>
        /// Could not find '{0}' in the feature list.
        /// </summary>
        internal static string MiddlewareFilterBuilder_NoMiddlewareFeature
        {
            get => GetString("MiddlewareFilterBuilder_NoMiddlewareFeature");
        }

        /// <summary>
        /// Could not find '{0}' in the feature list.
        /// </summary>
        internal static string FormatMiddlewareFilterBuilder_NoMiddlewareFeature(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddlewareFilterBuilder_NoMiddlewareFeature"), p0);

        /// <summary>
        /// The '{0}' property cannot be null.
        /// </summary>
        internal static string MiddlewareFilterBuilder_NullApplicationBuilder
        {
            get => GetString("MiddlewareFilterBuilder_NullApplicationBuilder");
        }

        /// <summary>
        /// The '{0}' property cannot be null.
        /// </summary>
        internal static string FormatMiddlewareFilterBuilder_NullApplicationBuilder(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddlewareFilterBuilder_NullApplicationBuilder"), p0);

        /// <summary>
        /// The '{0}' method in the type '{1}' must have a return type of '{2}'.
        /// </summary>
        internal static string MiddlewareFilter_InvalidConfigureReturnType
        {
            get => GetString("MiddlewareFilter_InvalidConfigureReturnType");
        }

        /// <summary>
        /// The '{0}' method in the type '{1}' must have a return type of '{2}'.
        /// </summary>
        internal static string FormatMiddlewareFilter_InvalidConfigureReturnType(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddlewareFilter_InvalidConfigureReturnType"), p0, p1, p2);

        /// <summary>
        /// Could not resolve a service of type '{0}' for the parameter '{1}' of method '{2}' on type '{3}'.
        /// </summary>
        internal static string MiddlewareFilter_ServiceResolutionFail
        {
            get => GetString("MiddlewareFilter_ServiceResolutionFail");
        }

        /// <summary>
        /// Could not resolve a service of type '{0}' for the parameter '{1}' of method '{2}' on type '{3}'.
        /// </summary>
        internal static string FormatMiddlewareFilter_ServiceResolutionFail(object p0, object p1, object p2, object p3)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddlewareFilter_ServiceResolutionFail"), p0, p1, p2, p3);

        /// <summary>
        /// An {0} cannot be created without a valid instance of {1}.
        /// </summary>
        internal static string AuthorizeFilter_AuthorizationPolicyCannotBeCreated
        {
            get => GetString("AuthorizeFilter_AuthorizationPolicyCannotBeCreated");
        }

        /// <summary>
        /// An {0} cannot be created without a valid instance of {1}.
        /// </summary>
        internal static string FormatAuthorizeFilter_AuthorizationPolicyCannotBeCreated(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("AuthorizeFilter_AuthorizationPolicyCannotBeCreated"), p0, p1);

        /// <summary>
        /// The '{0}' cannot bind to a model of type '{1}'. Change the model type to '{2}' instead.
        /// </summary>
        internal static string FormCollectionModelBinder_CannotBindToFormCollection
        {
            get => GetString("FormCollectionModelBinder_CannotBindToFormCollection");
        }

        /// <summary>
        /// The '{0}' cannot bind to a model of type '{1}'. Change the model type to '{2}' instead.
        /// </summary>
        internal static string FormatFormCollectionModelBinder_CannotBindToFormCollection(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("FormCollectionModelBinder_CannotBindToFormCollection"), p0, p1, p2);

        /// <summary>
        /// '{0}' requires the response cache middleware.
        /// </summary>
        internal static string VaryByQueryKeys_Requires_ResponseCachingMiddleware
        {
            get => GetString("VaryByQueryKeys_Requires_ResponseCachingMiddleware");
        }

        /// <summary>
        /// '{0}' requires the response cache middleware.
        /// </summary>
        internal static string FormatVaryByQueryKeys_Requires_ResponseCachingMiddleware(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("VaryByQueryKeys_Requires_ResponseCachingMiddleware"), p0);

        /// <summary>
        /// A duplicate entry for library reference {0} was found. Please check that all package references in all projects use the same casing for the same package references.
        /// </summary>
        internal static string CandidateResolver_DifferentCasedReference
        {
            get => GetString("CandidateResolver_DifferentCasedReference");
        }

        /// <summary>
        /// A duplicate entry for library reference {0} was found. Please check that all package references in all projects use the same casing for the same package references.
        /// </summary>
        internal static string FormatCandidateResolver_DifferentCasedReference(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CandidateResolver_DifferentCasedReference"), p0);

        /// <summary>
        /// Unable to create an instance of type '{0}'. The type specified in {1} must not be abstract and must have a parameterless constructor.
        /// </summary>
        internal static string MiddlewareFilterConfigurationProvider_CreateConfigureDelegate_CannotCreateType
        {
            get => GetString("MiddlewareFilterConfigurationProvider_CreateConfigureDelegate_CannotCreateType");
        }

        /// <summary>
        /// Unable to create an instance of type '{0}'. The type specified in {1} must not be abstract and must have a parameterless constructor.
        /// </summary>
        internal static string FormatMiddlewareFilterConfigurationProvider_CreateConfigureDelegate_CannotCreateType(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("MiddlewareFilterConfigurationProvider_CreateConfigureDelegate_CannotCreateType"), p0, p1);

        /// <summary>
        /// '{0}' and '{1}' are out of bounds for the string.
        /// </summary>
        internal static string Argument_InvalidOffsetLength
        {
            get => GetString("Argument_InvalidOffsetLength");
        }

        /// <summary>
        /// '{0}' and '{1}' are out of bounds for the string.
        /// </summary>
        internal static string FormatArgument_InvalidOffsetLength(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("Argument_InvalidOffsetLength"), p0, p1);

        /// <summary>
        /// Could not create an instance of type '{0}'. Model bound complex types must not be abstract or value types and must have a parameterless constructor.
        /// </summary>
        internal static string ComplexTypeModelBinder_NoParameterlessConstructor_ForType
        {
            get => GetString("ComplexTypeModelBinder_NoParameterlessConstructor_ForType");
        }

        /// <summary>
        /// Could not create an instance of type '{0}'. Model bound complex types must not be abstract or value types and must have a parameterless constructor.
        /// </summary>
        internal static string FormatComplexTypeModelBinder_NoParameterlessConstructor_ForType(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ComplexTypeModelBinder_NoParameterlessConstructor_ForType"), p0);

        /// <summary>
        /// Could not create an instance of type '{0}'. Model bound complex types must not be abstract or value types and must have a parameterless constructor. Alternatively, set the '{1}' property to a non-null value in the '{2}' constructor.
        /// </summary>
        internal static string ComplexTypeModelBinder_NoParameterlessConstructor_ForProperty
        {
            get => GetString("ComplexTypeModelBinder_NoParameterlessConstructor_ForProperty");
        }

        /// <summary>
        /// Could not create an instance of type '{0}'. Model bound complex types must not be abstract or value types and must have a parameterless constructor. Alternatively, set the '{1}' property to a non-null value in the '{2}' constructor.
        /// </summary>
        internal static string FormatComplexTypeModelBinder_NoParameterlessConstructor_ForProperty(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("ComplexTypeModelBinder_NoParameterlessConstructor_ForProperty"), p0, p1, p2);

        /// <summary>
        /// No page named '{0}' matches the supplied values.
        /// </summary>
        internal static string NoRoutesMatchedForPage
        {
            get => GetString("NoRoutesMatchedForPage");
        }

        /// <summary>
        /// No page named '{0}' matches the supplied values.
        /// </summary>
        internal static string FormatNoRoutesMatchedForPage(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("NoRoutesMatchedForPage"), p0);

        /// <summary>
        /// The relative page path '{0}' can only be used while executing a Razor Page. Specify a root relative path with a leading '/' to generate a URL outside of a Razor Page.
        /// </summary>
        internal static string UrlHelper_RelativePagePathIsNotSupported
        {
            get => GetString("UrlHelper_RelativePagePathIsNotSupported");
        }

        /// <summary>
        /// The relative page path '{0}' can only be used while executing a Razor Page. Specify a root relative path with a leading '/' to generate a URL outside of a Razor Page.
        /// </summary>
        internal static string FormatUrlHelper_RelativePagePathIsNotSupported(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("UrlHelper_RelativePagePathIsNotSupported"), p0);

        /// <summary>
        /// One or more validation errors occurred.
        /// </summary>
        internal static string ValidationProblemDescription_Title
        {
            get => GetString("ValidationProblemDescription_Title");
        }

        /// <summary>
        /// One or more validation errors occurred.
        /// </summary>
        internal static string FormatValidationProblemDescription_Title()
            => GetString("ValidationProblemDescription_Title");

        /// <summary>
        /// Action '{0}' does not have an attribute route. Action methods on controllers annotated with {1} must be attribute routed.
        /// </summary>
        internal static string ApiController_AttributeRouteRequired
        {
            get => GetString("ApiController_AttributeRouteRequired");
        }

        /// <summary>
        /// Action '{0}' does not have an attribute route. Action methods on controllers annotated with {1} must be attribute routed.
        /// </summary>
        internal static string FormatApiController_AttributeRouteRequired(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ApiController_AttributeRouteRequired"), p0, p1);

        /// <summary>
        /// No file provider has been configured to process the supplied file.
        /// </summary>
        internal static string VirtualFileResultExecutor_NoFileProviderConfigured
        {
            get => GetString("VirtualFileResultExecutor_NoFileProviderConfigured");
        }

        /// <summary>
        /// No file provider has been configured to process the supplied file.
        /// </summary>
        internal static string FormatVirtualFileResultExecutor_NoFileProviderConfigured()
            => GetString("VirtualFileResultExecutor_NoFileProviderConfigured");

        /// <summary>
        /// Type {0} specified by {1} is invalid. Type specified by {1} must derive from {2}.
        /// </summary>
        internal static string ApplicationPartFactory_InvalidFactoryType
        {
            get => GetString("ApplicationPartFactory_InvalidFactoryType");
        }

        /// <summary>
        /// Type {0} specified by {1} is invalid. Type specified by {1} must derive from {2}.
        /// </summary>
        internal static string FormatApplicationPartFactory_InvalidFactoryType(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("ApplicationPartFactory_InvalidFactoryType"), p0, p1, p2);

        /// <summary>
        /// {0} specified on {1} cannot be self referential.
        /// </summary>
        internal static string RelatedAssemblyAttribute_AssemblyCannotReferenceSelf
        {
            get => GetString("RelatedAssemblyAttribute_AssemblyCannotReferenceSelf");
        }

        /// <summary>
        /// {0} specified on {1} cannot be self referential.
        /// </summary>
        internal static string FormatRelatedAssemblyAttribute_AssemblyCannotReferenceSelf(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("RelatedAssemblyAttribute_AssemblyCannotReferenceSelf"), p0, p1);

        /// <summary>
        /// Related assembly '{0}' specified by assembly '{1}' could not be found in the directory {2}. Related assemblies must be co-located with the specifying assemblies.
        /// </summary>
        internal static string RelatedAssemblyAttribute_CouldNotBeFound
        {
            get => GetString("RelatedAssemblyAttribute_CouldNotBeFound");
        }

        /// <summary>
        /// Related assembly '{0}' specified by assembly '{1}' could not be found in the directory {2}. Related assemblies must be co-located with the specifying assemblies.
        /// </summary>
        internal static string FormatRelatedAssemblyAttribute_CouldNotBeFound(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("RelatedAssemblyAttribute_CouldNotBeFound"), p0, p1, p2);

        /// <summary>
        /// Each related assembly must be declared by exactly one assembly. The assembly '{0}' was declared as related assembly by the following:
        /// </summary>
        internal static string ApplicationAssembliesProvider_DuplicateRelatedAssembly
        {
            get => GetString("ApplicationAssembliesProvider_DuplicateRelatedAssembly");
        }

        /// <summary>
        /// Each related assembly must be declared by exactly one assembly. The assembly '{0}' was declared as related assembly by the following:
        /// </summary>
        internal static string FormatApplicationAssembliesProvider_DuplicateRelatedAssembly(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ApplicationAssembliesProvider_DuplicateRelatedAssembly"), p0);

        /// <summary>
        /// Assembly '{0}' declared as a related assembly by assembly '{1}' cannot define additional related assemblies.
        /// </summary>
        internal static string ApplicationAssembliesProvider_RelatedAssemblyCannotDefineAdditional
        {
            get => GetString("ApplicationAssembliesProvider_RelatedAssemblyCannotDefineAdditional");
        }

        /// <summary>
        /// Assembly '{0}' declared as a related assembly by assembly '{1}' cannot define additional related assemblies.
        /// </summary>
        internal static string FormatApplicationAssembliesProvider_RelatedAssemblyCannotDefineAdditional(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ApplicationAssembliesProvider_RelatedAssemblyCannotDefineAdditional"), p0, p1);

        /// <summary>
        /// Could not create an instance of type '{0}'. Model bound complex types must not be abstract or value types and must have a parameterless constructor. Alternatively, give the '{1}' parameter a non-null default value.
        /// </summary>
        internal static string ComplexTypeModelBinder_NoParameterlessConstructor_ForParameter
        {
            get => GetString("ComplexTypeModelBinder_NoParameterlessConstructor_ForParameter");
        }

        /// <summary>
        /// Could not create an instance of type '{0}'. Model bound complex types must not be abstract or value types and must have a parameterless constructor. Alternatively, give the '{1}' parameter a non-null default value.
        /// </summary>
        internal static string FormatComplexTypeModelBinder_NoParameterlessConstructor_ForParameter(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ComplexTypeModelBinder_NoParameterlessConstructor_ForParameter"), p0, p1);

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}