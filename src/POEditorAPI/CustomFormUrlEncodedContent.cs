namespace POEditorAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This is a custom re-implementation of FormUrlEncodedContent that allows for longer data strings.
    /// </summary>
    class CustomFormUrlEncodedContent : HttpContent
    {
        protected const int MaxLengthAllowed = 32765;

        private readonly byte[] _content;
        private readonly int _offset;
        private readonly int _count;

        public CustomFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base()
        {
            this._content = GetContentByteArray(nameValueCollection);
            this._offset = 0;
            this._count = this._content.Length;

            this.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return stream.WriteAsync(this._content, this._offset, this._count);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = this._count;
            return true;
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            return Task.FromResult<Stream>(new MemoryStream(this._content, this._offset, this._count, writable: false));
        }

        /// <summary>
        /// Escape the string by chunking it first.
        /// </summary>
        /// <param name="input">The string to escape</param>
        /// <returns>The escaped string</returns>
        private static string EscapeDataString(string input)
        {
            var sb = new StringBuilder();
            var loops = input.Length / MaxLengthAllowed;

            for (var i = 0; i <= loops; i++)
            {
                var block = i < loops
                                ? input.Substring(MaxLengthAllowed * i, MaxLengthAllowed)
                                : input.Substring(MaxLengthAllowed * i);

                sb.Append(Uri.EscapeDataString(block));
            }

            return sb.ToString();
        }


        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException(nameof(nameValueCollection));
            }

            // Encode and concatenate data
            var builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in nameValueCollection)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.Append(Encode(pair.Key));
                builder.Append('=');
                builder.Append(Encode(pair.Value));
            }

            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            // Escape spaces as '+'.
            return EscapeDataString(data).Replace("%20", "+");
        }
    }
}