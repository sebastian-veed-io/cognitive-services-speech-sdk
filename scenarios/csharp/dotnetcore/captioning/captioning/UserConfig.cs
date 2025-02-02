//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Captioning
{
    public class UserConfig
    {
        /// True to use a compressed audio format; otherwise, use uncompressed (wav).
        readonly public bool useCompressedAudio = false;
        /// Compressed audio format for input audio. Default is Any.
        readonly public AudioStreamContainerFormat compressedAudioFormat = AudioStreamContainerFormat.ANY;
        /// Whether to show, remove, or mask profanity. Default is to mask it.
        readonly public ProfanityOption profanityOption = ProfanityOption.Masked;
        /// Enable language identification for these languages.
        /// Languages must be delimited by commas.
        /// Example: en-US,ja-JP
        readonly public string[]? languageIDLanguages;
        /// Input audio file path. Default input is the microphone.
        readonly public string? inputFilePath;
        /// Output file path. Default output is the console.
        readonly public string? outputFilePath;
        /// Predefined phrases to help the Speech service interpret the provided audio.
        /// Phrases must be delimited by semicolons.
        /// Example: Constoso;Jessie;Rehaan
        readonly public string? phraseList;
        /// True to suppress console output (except errors). If true, this overrides showRecognizingResults.
        readonly public bool suppressConsoleOutput = false;
        /// True to output Recognizing results. Default is to output Recognized results only.
        /// Recognizing results are always written to the console, never to an output file.
        /// If suppressConsoleOutput is true, it overrides this setting.
        readonly public bool showRecognizingResults = false;
        /// Output captions in SubRip Text format (default is WebVTT format).
        readonly public bool useSubRipTextCaptionFormat = false;
        /// Set the stable partial result threshold on the Speech service. This setting value must contain an integer.
        /// Example: 3
        readonly public string? stablePartialResultThreshold;
        /// The subscription key for your Speech service subscription.
        readonly public string subscriptionKey;
        /// The region for your Speech service subscription.
        readonly public string region;
        
        public UserConfig (
            bool useCompressedAudio,
            AudioStreamContainerFormat compressedAudioFormat,
            ProfanityOption profanityOption,
            string[]? languageIDLanguages,
            string? inputFilePath,
            string? outputFilePath,
            string? phraseList,
            bool suppressConsoleOutput,
            bool showRecognizingResults,
            bool useSubRipTextCaptionFormat,
            string? stablePartialResultThreshold,
            string subscriptionKey,
            string region
            )
        {
            this.useCompressedAudio = useCompressedAudio;
            this.compressedAudioFormat = compressedAudioFormat;
            this.profanityOption = profanityOption;
            this.languageIDLanguages = languageIDLanguages;
            this.inputFilePath = inputFilePath;
            this.outputFilePath = outputFilePath;
            this.phraseList = phraseList;
            this.suppressConsoleOutput = suppressConsoleOutput;
            this.showRecognizingResults = showRecognizingResults;
            this.useSubRipTextCaptionFormat = useSubRipTextCaptionFormat;
            this.stablePartialResultThreshold = stablePartialResultThreshold;
            this.subscriptionKey = subscriptionKey;
            this.region = region;
        }
    }
}
