using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    public interface ITrinityAttachment
    {
        /// <summary>
        /// Name of file<br/>
        /// e.g.: "MyCoolFile.JPG"
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// File description, if applicable<br/>
        /// e.g.: "A picture of my dog"
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Url of attachment, if applicable <br/>
        /// e.g.: "https://autumn.revolt.chat/attachments/REDACTED/MyCoolFile.JPG"
        /// </summary>
        string? Url { get; }

        /// <summary>
        /// Proxy url of attachment, if applicable
        /// </summary>
        string? ProxyUrl { get; }

        /// <summary>
        /// Get a stream to the file, MAY RETURN NULL IF A SPECIFIC OBJECT LIKE A HTTPCLIENT IS NEEDED
        /// </summary>
        /// <returns>a System.IO.Stream <strong>OR NULL</strong> </returns>
        Stream GetStream();
    }
}