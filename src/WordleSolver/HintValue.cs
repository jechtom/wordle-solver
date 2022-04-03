namespace WordleSolver
{
    public enum HintValue : byte
    {
        /// <summary>
        /// Letter is not present in guessed word.
        /// </summary>
        GrayAndNotPresent,

        /// <summary>
        /// Letter is present in guessed word, but not in this location. Correct location(s) is/are already marked as green.
        /// </summary>
        GrayAndPresentOnOtherLocationAsGreen,

        /// <summary>
        /// Letter is present in guessed word, but not in this location. Correct location(s) is/are not discovered yet.
        /// </summary>
        Yellow,

        /// <summary>
        /// Letter is present and on correct location.
        /// </summary>
        Green
    }
}