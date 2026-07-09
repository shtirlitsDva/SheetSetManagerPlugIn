using System;
using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Validation outcome for one input field. <see cref="Ok"/> means the value is acceptable.
    /// </summary>
    public readonly record struct RenameInputValidation(bool Ok, string? Error)
    {
        public static RenameInputValidation Valid => new(true, null);
        public static RenameInputValidation Invalid(string message) => new(false, message);
    }

    /// <summary>
    /// Definition of one user-facing input on a rename profile. Profiles publish a list of
    /// these so the WPF panel can render the editor without per-profile XAML.
    /// </summary>
    public abstract class RenameInputField
    {
        protected RenameInputField(string name, string label, string? defaultValue)
        {
            Name = name;
            Label = label;
            DefaultValue = defaultValue;
        }

        /// <summary>Stable identifier used as a token in templates and as a settings key.</summary>
        public string Name { get; }

        /// <summary>Localized label shown next to the editor.</summary>
        public string Label { get; }

        /// <summary>Value used when the user has no persisted value for this field.</summary>
        public string? DefaultValue { get; }

        public abstract RenameInputValidation Validate(string? value);
    }

    /// <summary>Plain free-text input with an optional regex validator.</summary>
    public sealed class TextRenameInputField : RenameInputField
    {
        private readonly Func<string?, RenameInputValidation>? _validator;

        public TextRenameInputField(
            string name,
            string label,
            string? defaultValue = null,
            Func<string?, RenameInputValidation>? validator = null) : base(name, label, defaultValue)
        {
            _validator = validator;
        }

        public override RenameInputValidation Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return RenameInputValidation.Invalid($"{Label} is required.");
            return _validator?.Invoke(value) ?? RenameInputValidation.Valid;
        }
    }

    /// <summary>One choice in a <see cref="ChoiceRenameInputField"/>.</summary>
    public sealed record RenameChoice(string Value, string Label);

    /// <summary>Closed-set dropdown, e.g. the VF SeparatorStyle picker.</summary>
    public sealed class ChoiceRenameInputField : RenameInputField
    {
        public ChoiceRenameInputField(
            string name,
            string label,
            IReadOnlyList<RenameChoice> choices,
            string? defaultValue = null) : base(name, label, defaultValue ?? choices[0].Value)
        {
            Choices = choices;
        }

        public IReadOnlyList<RenameChoice> Choices { get; }

        public override RenameInputValidation Validate(string? value)
        {
            if (value == null)
                return RenameInputValidation.Invalid($"{Label} is required.");
            foreach (var c in Choices)
                if (c.Value == value) return RenameInputValidation.Valid;
            return RenameInputValidation.Invalid($"{Label} value '{value}' is not one of the allowed choices.");
        }
    }
}
