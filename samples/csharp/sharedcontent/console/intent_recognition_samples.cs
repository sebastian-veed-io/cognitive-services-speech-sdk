//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

// <toplevel>
using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
// </toplevel>

namespace MicrosoftSpeechSDKSamples
{
    public class IntentRecognitionSamples
    {
        /// <summary>
        /// Intent recognition using microphone. 
        /// </summary>
        public static async Task RecognitionWithMicrophoneAsync()
        {
            // <intentRecognitionWithMicrophone>
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription("YourLanguageUnderstandingSubscriptionKey", "YourLanguageUnderstandingServiceRegion");

            // Creates an intent recognizer using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Language Understanding model using the app id, and adds specific intents from your model
                var model = LanguageUnderstandingModel.FromAppId("YourLanguageUnderstandingAppId");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName1", "id1");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName2", "id2");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName3", "any-IntentId-here");

                // Starts recognizing.
                Console.WriteLine("Say something...");

                // Starts intent recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent Id: {result.IntentId}.");
                    Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
            // </intentRecognitionWithMicrophone>
        }

        /// <summary>
        /// Continuous intent recognition using file input. 
        /// </summary>
        public static async Task ContinuousRecognitionWithFileAsync()
        {
            // <intentContinuousRecognitionWithFile>
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("YourLanguageUnderstandingSubscriptionKey", "YourLanguageUnderstandingServiceRegion");

            // Creates an intent recognizer using file as audio input.
            // Replace with your own audio file name.
            using (var audioInput = AudioConfig.FromWavFileInput("whatstheweatherlike.wav"))
            {
                using (var recognizer = new IntentRecognizer(config, audioInput))
                {
                    // The TaskCompletionSource to stop recognition.
                    var stopRecognition = new TaskCompletionSource<int>();

                    // Creates a Language Understanding model using the app id, and adds specific intents from your model
                    var model = LanguageUnderstandingModel.FromAppId("YourLanguageUnderstandingAppId");
                    recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName1", "id1");
                    recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName2", "id2");
                    recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName3", "any-IntentId-here");

                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) => {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    };

                    recognizer.Recognized += (s, e) => {
                        if (e.Result.Reason == ResultReason.RecognizedIntent)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                            Console.WriteLine($"    Intent Id: {e.Result.IntentId}.");
                            Console.WriteLine($"    Language Understanding JSON: {e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                        }
                        else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                            Console.WriteLine($"    Intent not recognized.");
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) => {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) => {
                        Console.WriteLine("\n    Session started event.");
                    };

                    recognizer.SessionStopped += (s, e) => {
                        Console.WriteLine("\n    Session stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };


                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    Console.WriteLine("Say something...");
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
            // </intentContinuousRecognitionWithFile>
        }

        /// <summary>
        /// Intent recognition in the German, using microphone.
        /// </summary>
        public static async Task RecognitionWithMicrophoneUsingLanguageAsync()
        {
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("YourLanguageUnderstandingSubscriptionKey", "YourLanguageUnderstandingServiceRegion");
            var language = "de-de";
            config.SpeechRecognitionLanguage = language;

            // Creates an intent recognizer in the specified language using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Language Understanding model using the app id, and adds specific intents from your model
                var model = LanguageUnderstandingModel.FromAppId("YourLanguageUnderstandingAppId");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName1", "id1");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName2", "id2");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName3", "any-IntentId-here");

                // Starts recognizing.
                Console.WriteLine("Say something in " + language + "...");

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent Id: {result.IntentId}.");
                    Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }

        /// <summary>
        /// Use pattern matching for intent recognition from your default microphone input
        /// </summary>
        public static async Task IntentPatternMatchingWithMicrophoneAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service
            // region. Note that in contrast to the other samples this DOES NOT require a LUIS
            // application.
            // The default recognition language is "en-us".
            var config = SpeechConfig.FromSubscription("YourLanguageUnderstandingSubscriptionKey", "YourLanguageUnderstandingServiceRegion");

            // Creates an intent recognizer using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Pattern Matching model and adds specific intents from your model. The
                // Id is used to identify this model from others in the collection.
                var model = new PatternMatchingModel("YourPatternMatchingModelId");

                // Creates a pattern that uses groups of optional words. "[Go | Take me]" will match either "Go", "Take me", or "".
                var patternWithOptionalWords = "[Go | Take me] to [floor|level] {floorName}";

                // Creates a pattern that uses an optional entity and group that could be used to tie commands together.
                var patternWithOptionalEntity = "Go to parking [{parkingLevel}]";

                // You can also have multiple entities of the same name in a single pattern by adding appending a unique identifier
                // to distinguish between the instances. For example:
                var patternWithTwoOfTheSameEntity = "Go to floor {floorName:1} [and then go to floor {floorName:2}]";
                // NOTE: Both floorName:1 and floorName:2 are tied to the same list of entries. The identifier can be a string
                //       and is separated from the entity name by a ':'

                // Adds some intents to look for specific patterns.
                model.Intents.Add(new PatternMatchingIntent("ChangeFloors", patternWithOptionalWords, patternWithOptionalEntity, patternWithTwoOfTheSameEntity));
                model.Intents.Add(new PatternMatchingIntent("DoorControl", "{action} the doors", "{action} doors", "{action} the door", "{action} door"));

                // Creates the "floorName" entity and set it to type list.
                // Adds acceptable values. NOTE the default entity type is Any and so we do not need
                // to declare the "action" entity.
                model.Entities.Add(PatternMatchingEntity.CreateListEntity("floorName", EntityMatchMode.Strict, "ground floor", "lobby", "1st", "first", "one", "1", "2nd", "second", "two", "2"));

                // Creates the "parkingLevel" entity as a pre-built integer
                model.Entities.Add(PatternMatchingEntity.CreateIntegerEntity("parkingLevel"));

                // Add the model to a new language model collection
                var modelCollection = new LanguageUnderstandingModelCollection();
                modelCollection.Add(model);

                // Apply the language model collection to the recognizer.
                recognizer.ApplyLanguageModels(modelCollection);

                Console.WriteLine("Say something...");

                // Starts intent recognition, and returns after a single utterance is recognized. The
                // end of a single utterance is determined by listening for silence at the end, or until
                // a maximum of 15 seconds of audio is processed. The task returns the recognition
                // text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only
                // for single shot recognition like command or query.
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync()
                // instead.
                var result = await recognizer.RecognizeOnceAsync();

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"       Intent Id={result.IntentId}.");

                    var entities = result.Entities;
                    switch (result.IntentId)
                    {
                        case "ChangeFloors":
                            if (entities.TryGetValue("floorName", out string floorName))
                            {
                                Console.WriteLine($"       FloorName={floorName}");
                            }

                            if (entities.TryGetValue("floorName:1", out floorName))
                            {
                                Console.WriteLine($"     FloorName:1={floorName}");
                            }

                            if (entities.TryGetValue("floorName:2", out floorName))
                            {
                                Console.WriteLine($"     FloorName:2={floorName}");
                            }

                            if (entities.TryGetValue("parkingLevel", out string parkingLevel))
                            {
                                Console.WriteLine($"    ParkingLevel={parkingLevel}");
                            }

                            break;

                        case "DoorControl":
                            if (entities.TryGetValue("action", out string action))
                            {
                                Console.WriteLine($"          Action={action}");
                            }
                            break;
                    }
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }
    }
}
