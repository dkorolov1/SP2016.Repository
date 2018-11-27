using System;
using System.Runtime.Serialization;

namespace SP2016.Repository.Mapping
{
    /// <summary>
    /// Exception that can occur when mapping a field of an SPListItem to a property of a business entity. 
    /// </summary>
    [Serializable]
    public class ListItemFieldMappingException : Exception
    {
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemFieldMappingException"/> class.
        /// </summary>
        public ListItemFieldMappingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemFieldMappingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ListItemFieldMappingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemFieldMappingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ListItemFieldMappingException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemFieldMappingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ListItemFieldMappingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
