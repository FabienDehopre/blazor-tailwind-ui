namespace BlazorTailwindUI.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Routing;

    public partial class NavMenuLink : IDisposable
    {
        private bool isActive;
        private string? hrefAbsolute;
        private string? cssClass;

        [Parameter]
        public RenderFragment? Label { get; set; }

        [Parameter]
        public RenderFragment<(bool isDesktop, bool isActive)>? Icon { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        [Parameter]
        public NavLinkMatch Match { get; set; }

        [Parameter]
        public bool IsDesktop { get; set; }

        [Parameter]
        public string? LabelText { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        protected override void OnInitialized() => this.NavigationManager.LocationChanged += this.OnLocationChanged;

        protected override void OnParametersSet()
        {
            var href = (string?)null;
            if (this.AdditionalAttributes != null && this.AdditionalAttributes.TryGetValue("href", out var obj))
            {
                href = Convert.ToString(obj);
            }

            this.hrefAbsolute = href == null ? null : this.NavigationManager.ToAbsoluteUri(href).AbsoluteUri;
            this.isActive = this.ShouldMatch(this.NavigationManager.Uri);
            this.cssClass = (string?)null;
            if (this.AdditionalAttributes != null && this.AdditionalAttributes.TryGetValue("class", out obj))
            {
                this.cssClass = Convert.ToString(obj);
            }

            this.OnLocationChanged(this, new LocationChangedEventArgs(this.NavigationManager.Uri, true));
        }

        public void Dispose() => this.NavigationManager.LocationChanged -= this.OnLocationChanged;

        private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
        {
            var shouldBeActiveNow = this.ShouldMatch(args.Location);
            if (shouldBeActiveNow != this.isActive)
            {
                this.isActive = shouldBeActiveNow;
                this.StateHasChanged();
            }
        }

        private bool ShouldMatch(string currentUriAbsolute)
        {
            if (this.hrefAbsolute == null)
            {
                return false;
            }

            if (this.EqualsHrefExactlyOrIfTrailingSlashAdded(currentUriAbsolute))
            {
                return true;
            }

            if (this.Match == NavLinkMatch.Prefix && this.IsStrictlyPrefixWithSeparator(currentUriAbsolute, this.hrefAbsolute))
            {
                return true;
            }

            return false;
        }

        private bool EqualsHrefExactlyOrIfTrailingSlashAdded(string currentUriAbsolute)
        {
            Debug.Assert(this.hrefAbsolute != null);

            if (string.Equals(currentUriAbsolute, this.hrefAbsolute, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (currentUriAbsolute.Length == this.hrefAbsolute.Length - 1)
            {
                // Special case: highlight links to http://host/path/ even if you're
                // at http://host/path (with no trailing slash)
                //
                // This is because the router accepts an absolute URI value of "same
                // as base URI but without trailing slash" as equivalent to "base URI",
                // which in turn is because it's common for servers to return the same page
                // for http://host/vdir as they do for host://host/vdir/ as it's no
                // good to display a blank page in that case.
                if (this.hrefAbsolute[^1] == '/' && this.hrefAbsolute.StartsWith(currentUriAbsolute, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsStrictlyPrefixWithSeparator(string value, string prefix)
        {
            var prefixLength = prefix.Length;
            if (value.Length > prefixLength)
            {
                return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    && (
                        // Only match when there's a separator character either at the end of the
                        // prefix or right after it.
                        // Example: "/abc" is treated as a prefix of "/abc/def" but not "/abcdef"
                        // Example: "/abc/" is treated as a prefix of "/abc/def" but not "/abcdef"
                        prefixLength == 0
                        || !char.IsLetterOrDigit(prefix[prefixLength - 1])
                        || !char.IsLetterOrDigit(value[prefixLength])
                    );
            }

            return false;
        }
    }
}
