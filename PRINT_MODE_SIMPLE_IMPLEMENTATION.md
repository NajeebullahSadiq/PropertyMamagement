# Simple Print Mode Implementation

Since the modal dialog approach is having issues, here's a simpler implementation that adds buttons at the top of the print page.

## Simple Approach

Instead of a modal dialog, add two buttons at the top of the page that are hidden when printing.

### Implementation Steps:

1. Add buttons at the top of the print page (always visible, not conditional)
2. Use `@media print` to hide the buttons when printing
3. Apply data-only class when button is clicked

This approach is simpler and doesn't depend on complex state management.

## Alternative: Use URL Parameter

The easiest solution is to use URL parameters:

- For full design: `/print/2` (default)
- For data only: `/print/2?mode=data-only`

Then update the print button in the list to show two options or ask user before navigating.

## Recommendation

For immediate use, I recommend using the URL parameter approach:

1. In the property/vehicle list, change the print button to show a dropdown or two buttons
2. One button navigates to `/print/ID` (full design)
3. Other button navigates to `/print/ID?mode=data-only` (data only)

This is the simplest and most reliable approach.
