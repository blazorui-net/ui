# BlazorUI.Components

Pre-styled Blazor components with shadcn/ui design. Beautiful defaults with zero configuration - no Tailwind setup required!

## Features

- **Zero Configuration**: Pre-built CSS included - no Tailwind setup required
- **shadcn/ui Design**: Beautiful, modern design language inspired by shadcn/ui
- **Pre-Styled Components**: Production-ready components with pre-built styling
- **Dark Mode**: Built-in dark mode support using CSS variables
- **shadcn/ui Theme Compatible**: Use any theme from shadcn/ui or tweakcn.com
- **Fully Customizable**: Override styles with custom CSS or Tailwind classes
- **Accessible**: Built on BlazorUI.Primitives with WCAG 2.1 AA compliance
- **Composable**: Flexible component composition patterns
- **Type-Safe**: Full C# type safety with IntelliSense support
- **.NET 8**: Built for the latest .NET platform

## Installation

```bash
dotnet add package BlazorUI.Components
```

This package automatically includes:
- `BlazorUI.Primitives` - Headless primitives providing behavior and accessibility
- Pre-built CSS - No Tailwind setup required!

## Quick Start

### 1. Add to your `_Imports.razor`:

```razor
@using BlazorUI.Components
```

### 2. Add CSS to your `App.razor`:

BlazorUI Components come with pre-built CSS - no Tailwind setup required!

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />

    <!-- Optional: Your custom theme (defines CSS variables) -->
    <link rel="stylesheet" href="styles/theme.css" />

    <!-- Pre-built BlazorUI styles (included in NuGet package) -->
    <link rel="stylesheet" href="_content/BlazorUI.Components/blazorui.css" />

    <HeadOutlet @rendermode="InteractiveServer" />
</head>
<body>
    <Routes @rendermode="InteractiveServer" />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

### 3. Start using components:

```razor
<Button Variant="ButtonVariant.Default">Click me</Button>

<Dialog>
    <DialogTrigger AsChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Welcome to BlazorUI</DialogTitle>
            <DialogDescription>
                Beautiful Blazor components with zero configuration
            </DialogDescription>
        </DialogHeader>
    </DialogContent>
</Dialog>
```

That's it! No Tailwind installation, no build configuration needed.

## Available Components

- **Accordion**: Collapsible content sections with smooth animations
- **Avatar**: User profile images with fallback initials and icons
- **Badge**: Labels for status, categories, and metadata
- **Button**: Interactive buttons with multiple variants and sizes
- **Checkbox**: Binary selection control with indeterminate state
- **Collapsible**: Expandable content area with trigger control
- **Combobox**: Autocomplete input with searchable dropdown
- **Command**: Command palette for quick actions and navigation
- **Dialog**: Modal dialogs with backdrop and focus management
- **Dropdown Menu**: Context menus with items, separators, and shortcuts
- **Hover Card**: Rich preview cards on hover with delay control
- **Input**: Text input fields with multiple types and sizes
- **Label**: Accessible labels for form controls
- **Popover**: Floating panels for additional content and actions
- **Radio Group**: Mutually exclusive options with keyboard navigation
- **Select**: Dropdown selection with groups and labels
- **Separator**: Visual dividers for content sections
- **Sheet**: Side panels that slide in from viewport edges
- **Sidebar**: Responsive navigation sidebar with collapsible menus
- **Skeleton**: Loading placeholders for content and images
- **Switch**: Toggle control for on/off states
- **Tabs**: Tabbed interface for organizing related content
- **Tooltip**: Brief informational popups on hover or focus

## Theming

BlazorUI is **100% compatible with shadcn/ui themes**. Customize your application's appearance using CSS variables.

### Using Themes from shadcn/ui and tweakcn

You can use any theme from:
- **[shadcn/ui themes](https://ui.shadcn.com/themes)** - Official shadcn/ui theme gallery
- **[tweakcn.com](https://tweakcn.com)** - Advanced theme customization tool

Simply copy the CSS variables and paste them into your `wwwroot/styles/theme.css` file.

### Example Theme

Create `wwwroot/styles/theme.css`:

```css
@layer base {
  :root {
    --background: oklch(1 0 0);
    --foreground: oklch(0.1450 0 0);
    --primary: oklch(0.2050 0 0);
    --primary-foreground: oklch(0.9850 0 0);
    /* ... other variables */
  }

  .dark {
    --background: oklch(0.1450 0 0);
    --foreground: oklch(0.9850 0 0);
    --primary: oklch(0.9220 0 0);
    --primary-foreground: oklch(0.2050 0 0);
    /* ... other variables */
  }
}
```

Reference it in your `App.razor` **before** the BlazorUI CSS:

```razor
<link rel="stylesheet" href="styles/theme.css" />
<link rel="stylesheet" href="_content/BlazorUI.Components/blazorui.css" />
```

### Dark Mode

Dark mode automatically activates when you add the `.dark` class to the `<html>` element. All components will switch to dark mode colors.

## Usage Example

```razor
@using BlazorUI.Components.Dialog

<Dialog>
    <DialogTrigger>Open Dialog</DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Confirm Action</DialogTitle>
            <DialogDescription>
                Are you sure you want to proceed?
            </DialogDescription>
        </DialogHeader>
        <p>This action cannot be undone.</p>
        <DialogFooter>
            <DialogClose>Cancel</DialogClose>
            <Button Variant="default">Confirm</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

## Button Variants

```razor
@using BlazorUI.Components.Button

<Button Variant="default">Default</Button>
<Button Variant="destructive">Destructive</Button>
<Button Variant="outline">Outline</Button>
<Button Variant="secondary">Secondary</Button>
<Button Variant="ghost">Ghost</Button>
<Button Variant="link">Link</Button>

<Button Size="sm">Small</Button>
<Button Size="default">Default</Button>
<Button Size="lg">Large</Button>
<Button Size="icon">
    <IconPlus />
</Button>
```

## Form Example

```razor
@using BlazorUI.Components.Label
@using BlazorUI.Components.Input
@using BlazorUI.Components.Checkbox
@using BlazorUI.Components.Button

<div class="space-y-4">
    <div>
        <Label For="email">Email</Label>
        <Input Id="email" Type="email" Placeholder="name@example.com" />
    </div>

    <div class="flex items-center space-x-2">
        <Checkbox Id="terms" @bind-Checked="agreedToTerms" />
        <Label For="terms">I agree to the terms and conditions</Label>
    </div>

    <Button Disabled="@(!agreedToTerms)">Submit</Button>
</div>

@code {
    private bool agreedToTerms = false;
}
```

## Customizing Components

### Override Default Styles

Use the `Class` parameter to add custom CSS classes or Tailwind classes (if you have Tailwind set up):

```razor
<Button Class="bg-purple-600 hover:bg-purple-700">
    Custom Button
</Button>

<Card Class="border-2 border-purple-500 shadow-xl">
    Custom Card Styling
</Card>
```

**Note:** BlazorUI Components include pre-built CSS and don't require Tailwind. However, you can still use Tailwind classes for customization if you've set up Tailwind in your project.

### Component Composition

Build complex UIs by composing components:

```razor
<Card>
    <CardHeader>
        <CardTitle>Settings</CardTitle>
        <CardDescription>Manage your account settings</CardDescription>
    </CardHeader>
    <CardContent class="space-y-4">
        <div>
            <Label>Email Notifications</Label>
            <Switch @bind-Checked="emailNotifications" />
        </div>
        <Separator />
        <div>
            <Label>Push Notifications</Label>
            <Switch @bind-Checked="pushNotifications" />
        </div>
    </CardContent>
    <CardFooter>
        <Button>Save Changes</Button>
    </CardFooter>
</Card>
```

## Design Philosophy

BlazorUI.Components follows the shadcn/ui philosophy with zero-configuration deployment:

1. **Zero Configuration**: Pre-built CSS included - just install and use
2. **shadcn/ui Compatible**: Uses the same design tokens and CSS variables
3. **Built on Primitives**: All behavior comes from BlazorUI.Primitives
4. **Theme Tokens**: Fully themeable using CSS variables
5. **Accessible by Default**: WCAG 2.1 AA compliance built-in
6. **Customizable**: Override with custom CSS or add Tailwind if needed

## When to Use

**Use BlazorUI.Components when:**
- Want beautiful defaults with shadcn/ui design
- Need zero-configuration setup (no build tools required)
- Want to ship quickly without building components from scratch
- Need dark mode and theming support out of the box
- Want shadcn/ui theme compatibility

**Consider [BlazorUI.Primitives](https://www.nuget.org/packages/BlazorUI.Primitives) when:**
- Building a completely custom design system
- Want zero opinions about styling
- Need to match a specific brand or design language
- Prefer full control over all CSS

## Documentation

For full documentation, examples, and API reference, visit:
- [Documentation Site](https://github.com/blazorui-net/ui)
- [Component Demos](https://github.com/blazorui-net/ui)
- [GitHub Repository](https://github.com/blazorui-net/ui)

## Dependencies

- [BlazorUI.Primitives](https://www.nuget.org/packages/BlazorUI.Primitives) - Headless component primitives (auto-installed)
- Pre-built CSS (included in package)
- No external dependencies required!

**Optional:**
- Tailwind CSS (if you want to use Tailwind classes for customization)

## License

MIT License - see [LICENSE](https://github.com/blazorui-net/ui/blob/main/LICENSE) for details.

## Contributing

Contributions are welcome! Please see our [Contributing Guide](https://github.com/blazorui-net/ui/blob/main/CONTRIBUTING.md).

---

Made with ❤️ by the BlazorUI team
