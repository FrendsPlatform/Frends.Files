﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.Files.Move.Definitions
{
    /// <summary>
    /// Input parameters.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Source directory.
        /// </summary>
        /// <example>c:\temp</example>
        [DisplayFormat(DataFormatString = "Text")]
        public string Directory { get; set; }

        /// <summary>
        /// Pattern to match for files. The file mask uses regular expressions, but for convenience, it has special handling for * and ? wildcards.
        /// </summary>
        /// <example>test.txt, test*.txt, test?.txt, test.(txt|xml), test.[^t][^x][^t], &lt;regex&gt;^(?!prof).*_test.txt</example>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"**\\Folder\\*.xml\"")]
        public string Pattern { get; set; }
    }
}