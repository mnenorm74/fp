﻿using System.Collections.Generic;
using System.Linq;
using TagsCloudVisualization.ErrorHandling;

namespace TagsCloudVisualization.WordSizing
{
    public class FrequencyWordSizer : IWordSizer
    {
        public Result<IEnumerable<SizedWord>> GetSizedWords(IEnumerable<string> words, int minSize = 10, int step = 5,
            int maxSize = 50)
        {
            if (minSize <= 0)
                return Result.Fail<IEnumerable<SizedWord>>("Min size must be positive");
            if (step <= 0)
                return Result.Fail<IEnumerable<SizedWord>>("Step size must be positive");
            var frequencyDictionary = GetFrequencyDictionary(words);
            var sortedFrequencyDictionary = frequencyDictionary.OrderBy(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            var sizedWords = new List<SizedWord>();
            var currentSize = minSize;
            var currentFrequency = sortedFrequencyDictionary.First().Value;
            foreach (var word in sortedFrequencyDictionary)
            {
                if (word.Value > currentFrequency)
                {
                    currentFrequency = word.Value;
                    currentSize = currentSize + step < maxSize ? currentSize + step : maxSize;
                }

                sizedWords.Add(new SizedWord(word.Key, currentSize));
            }

            sizedWords.Reverse();

            return Result.Ok(sizedWords.AsEnumerable());
        }

        private Dictionary<string, int> GetFrequencyDictionary(IEnumerable<string> words)
        {
            var frequencyDictionary = new Dictionary<string, int>();
            foreach (var word in words)
            {
                var lowerCaseWord = word.ToLower();
                if (frequencyDictionary.ContainsKey(lowerCaseWord))
                {
                    frequencyDictionary[lowerCaseWord]++;
                }
                else
                {
                    frequencyDictionary.Add(lowerCaseWord, 1);
                }
            }

            return frequencyDictionary;
        }
    }
}