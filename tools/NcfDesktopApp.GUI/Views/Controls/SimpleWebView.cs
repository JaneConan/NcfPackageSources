using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Platform;
using System.Net.Http;
using System.Text;

#if WINDOWS
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

namespace NcfDesktopApp.GUI.Views.Controls;

public partial class SimpleWebView : UserControl
{
    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<SimpleWebView, string>(nameof(Source), "");

    public string Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private Border _contentBorder = null!;
    private string _currentUrl = "";
    private static readonly HttpClient _httpClient = new();
    
#if WINDOWS
    private WebView2? _webView2;
    private bool _isWebView2Ready = false;
#endif
    
    private TextBlock _statusText = null!;
    private Button _refreshButton = null!;
    private Button _openExternalButton = null!;
    private bool _isWebViewReady = false;

    public SimpleWebView()
    {
        InitializeComponent();
        _ = InitializeWebViewAsync();
    }

    private void InitializeComponent()
    {
        var content = new StackPanel
        {
            Spacing = 15,
            Margin = new Thickness(20)
        };

        // 状态显示
        var statusArea = new Border
        {
            Background = Brushes.LightBlue,
            BorderBrush = Brushes.SteelBlue,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(20),
            MinHeight = 120
        };

        var statusContent = new StackPanel
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Spacing = 10
        };

        var iconText = new TextBlock
        {
            Text = "🌐",
            FontSize = 48,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };

        _statusText = new TextBlock
        {
            Text = "正在初始化内嵌浏览器...",
            FontSize = 16,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Foreground = Brushes.DarkSlateBlue
        };

        statusContent.Children.Add(iconText);
        statusContent.Children.Add(_statusText);
        statusArea.Child = statusContent;

        // 按钮区域
        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Spacing = 10,
            Margin = new Thickness(0, 15, 0, 0)
        };

        _refreshButton = new Button
        {
            Content = "🔄 刷新",
            Padding = new Thickness(15, 8),
            Background = Brushes.Green,
            Foreground = Brushes.White,
            CornerRadius = new CornerRadius(4),
            IsEnabled = false
        };
        _refreshButton.Click += OnRefreshClick;

        _openExternalButton = new Button
        {
            Content = "🌍 外部浏览器",
            Padding = new Thickness(15, 8),
            Background = Brushes.Orange,
            Foreground = Brushes.White,
            CornerRadius = new CornerRadius(4),
            IsEnabled = false
        };
        _openExternalButton.Click += OnOpenExternalClick;

        buttonPanel.Children.Add(_refreshButton);
        buttonPanel.Children.Add(_openExternalButton);

        content.Children.Add(statusArea);
        content.Children.Add(buttonPanel);

        _contentBorder = new Border
        {
            Background = Brushes.White,
            Child = content
        };

        Content = _contentBorder;
    }

    private async Task InitializeWebViewAsync()
    {
        try
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                try
                {
#if WINDOWS
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        await InitializeWebView2Async();
                    }
                    else
#endif
                    {
                        await InitializeEmbeddedViewAsync();
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"WebView初始化失败: {ex.Message}", Brushes.Red);
                    ShowFallbackView();
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"WebView初始化异常: {ex.Message}");
            ShowFallbackView();
        }
    }

#if WINDOWS
    private async Task InitializeWebView2Async()
    {
        try
        {
            UpdateStatus("正在初始化 WebView2...", Brushes.Blue);
            
            // 创建WebView2控件
            _webView2 = new WebView2();
            
            // 设置WebView2环境
            var userDataFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NcfDesktopApp"
            );
            
            var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await _webView2.EnsureCoreWebView2Async(environment);

            // 配置WebView2设置
            _webView2.CoreWebView2.Settings.IsWebMessageEnabled = true;
            _webView2.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
            _webView2.CoreWebView2.Settings.AreHostObjectsAllowed = true;
            _webView2.CoreWebView2.Settings.IsScriptEnabled = true;
            _webView2.CoreWebView2.Settings.AreDevToolsEnabled = true;
            
            // 事件处理
            _webView2.CoreWebView2.NavigationStarting += (s, e) =>
            {
                UpdateStatus($"正在加载: {e.Uri}", Brushes.Blue);
                OnNavigationStarted(e.Uri);
            };
            
            _webView2.CoreWebView2.NavigationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                {
                    UpdateStatus("✅ 页面加载完成", Brushes.Green);
                    OnNavigationCompleted(_currentUrl);
                }
                else
                {
                    UpdateStatus("❌ 页面加载失败", Brushes.Red);
                    OnNavigationFailed("导航失败");
                }
            };

            _webView2.CoreWebView2.DOMContentLoaded += async (s, e) =>
            {
                // 页面DOM加载完成
                UpdateStatus("页面内容已加载", Brushes.Green);
            };

            // 创建NativeControlHost来承载WebView2
            var nativeHost = new NativeControlHost
            {
                Content = _webView2
            };

            // 创建主容器
            var webViewContainer = new Grid();
            webViewContainer.Children.Add(nativeHost);
            
            // 替换内容
            _contentBorder.Child = webViewContainer;
            _isWebView2Ready = true;
            _isWebViewReady = true;
            
            UpdateStatus("🚀 WebView2 已就绪，可以显示动态网页", Brushes.Green);
            
            // 如果已经有URL，立即导航
            if (!string.IsNullOrEmpty(_currentUrl) && _currentUrl != "未启动")
            {
                await NavigateWebView2(_currentUrl);
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"WebView2 初始化失败: {ex.Message}", Brushes.Red);
            Debug.WriteLine($"WebView2 错误详情: {ex}");
            await InitializeEmbeddedViewAsync(); // 回退到嵌入视图
        }
    }

    private async Task NavigateWebView2(string url)
    {
        try
        {
            if (_webView2?.CoreWebView2 != null)
            {
                UpdateStatus($"正在导航到: {url}", Brushes.Blue);
                _webView2.CoreWebView2.Navigate(url);
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"导航失败: {ex.Message}", Brushes.Red);
            OnNavigationFailed(ex.Message);
        }
    }
#endif

    private async Task InitializeEmbeddedViewAsync()
    {
        try
        {
            UpdateStatus("正在创建内嵌预览...", Brushes.Orange);
            
            // 创建一个可以显示HTML内容的区域
            var embedContainer = new ScrollViewer
            {
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10)
            };

            var contentArea = new StackPanel
            {
                Spacing = 15
            };

            var welcomeText = new TextBlock
            {
                Text = "📱 移动端预览模式",
                FontSize = 18,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Foreground = Brushes.DarkBlue
            };

            var descText = new TextBlock
            {
                Text = "在此平台上，我们提供 NCF 应用的预览和管理功能。\n完整的网页体验请使用外部浏览器。",
                FontSize = 14,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 20)
            };

            contentArea.Children.Add(welcomeText);
            contentArea.Children.Add(descText);
            
            embedContainer.Content = contentArea;
            _contentBorder.Child = embedContainer;

            UpdateStatus("✅ 预览模式已就绪", Brushes.Green);
            _isWebViewReady = true;

            // 如果有URL，获取页面信息
            if (!string.IsNullOrEmpty(_currentUrl) && _currentUrl != "未启动")
            {
                await FetchPageInfoAsync(_currentUrl, contentArea);
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"创建预览失败: {ex.Message}", Brushes.Red);
            ShowFallbackView();
        }
    }

    private async Task FetchPageInfoAsync(string url, StackPanel container)
    {
        try
        {
            UpdateStatus("正在连接 NCF 应用...", Brushes.Blue);

            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(8));
            var response = await _httpClient.GetAsync(url, cts.Token);
            var content = await response.Content.ReadAsStringAsync();

            // 创建页面信息显示
            var infoPanel = new Border
            {
                Background = Brushes.LightYellow,
                BorderBrush = Brushes.Orange,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 10, 0, 0)
            };

            var infoText = new TextBlock
            {
                Text = ExtractPageInfo(content, response, url),
                FontSize = 13,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Foreground = Brushes.DarkOliveGreen
            };

            infoPanel.Child = infoText;
            container.Children.Add(infoPanel);

            // 添加快速操作按钮
            var actionPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 10,
                Margin = new Thickness(0, 15, 0, 0)
            };

            var openFullButton = new Button
            {
                Content = "🌍 完整访问",
                Background = Brushes.RoyalBlue,
                Foreground = Brushes.White,
                Padding = new Thickness(20, 10),
                CornerRadius = new CornerRadius(5)
            };
            openFullButton.Click += OnOpenExternalClick;

            var refreshInfoButton = new Button
            {
                Content = "🔄 刷新信息",
                Background = Brushes.SeaGreen,
                Foreground = Brushes.White,
                Padding = new Thickness(20, 10),
                CornerRadius = new CornerRadius(5)
            };
            refreshInfoButton.Click += OnRefreshClick;

            actionPanel.Children.Add(refreshInfoButton);
            actionPanel.Children.Add(openFullButton);
            container.Children.Add(actionPanel);

            UpdateStatus("✅ NCF 应用连接成功", Brushes.Green);
            OnNavigationCompleted(url);
        }
        catch (Exception ex)
        {
            var errorPanel = new Border
            {
                Background = Brushes.MistyRose,
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 10, 0, 0)
            };

            var errorText = new TextBlock
            {
                Text = $"❌ 连接失败：{ex.Message}\n\n建议：\n• 确认 NCF 应用正在运行\n• 检查端口号是否正确\n• 尝试使用外部浏览器访问",
                FontSize = 13,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Foreground = Brushes.DarkRed
            };

            errorPanel.Child = errorText;
            container.Children.Add(errorPanel);

            UpdateStatus("⚠️ 连接失败", Brushes.Red);
            OnNavigationFailed(ex.Message);
        }
    }

    private string ExtractPageInfo(string htmlContent, HttpResponseMessage response, string url)
    {
        var info = new StringBuilder();
        info.AppendLine($"🌐 NCF 应用状态报告");
        info.AppendLine($"📍 地址：{url}");
        info.AppendLine($"✅ 状态：{response.StatusCode} ({(int)response.StatusCode})");
        
        if (response.Content.Headers.ContentLength.HasValue)
        {
            var sizeKb = response.Content.Headers.ContentLength.Value / 1024.0;
            info.AppendLine($"📊 页面大小：{sizeKb:F1} KB");
        }

        // 检测框架
        var frameworks = new System.Collections.Generic.List<string>();
        if (htmlContent.Contains("bootstrap", StringComparison.OrdinalIgnoreCase))
            frameworks.Add("Bootstrap");
        if (htmlContent.Contains("jquery", StringComparison.OrdinalIgnoreCase))
            frameworks.Add("jQuery");
        if (htmlContent.Contains("vue", StringComparison.OrdinalIgnoreCase))
            frameworks.Add("Vue.js");
        if (htmlContent.Contains("angular", StringComparison.OrdinalIgnoreCase))
            frameworks.Add("Angular");

        if (frameworks.Count > 0)
        {
            info.AppendLine($"🎨 检测到：{string.Join(", ", frameworks)}");
        }

        info.AppendLine();
        info.AppendLine($"💡 提示：点击\"完整访问\"在外部浏览器中查看所有功能");

        return info.ToString();
    }

    private void ShowFallbackView()
    {
        var fallbackContent = new StackPanel
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Spacing = 15
        };

        fallbackContent.Children.Add(new TextBlock
        {
            Text = "⚠️",
            FontSize = 48,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });

        fallbackContent.Children.Add(new TextBlock
        {
            Text = "浏览器组件不可用",
            FontSize = 16,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });

        fallbackContent.Children.Add(new TextBlock
        {
            Text = "请使用外部浏览器访问 NCF 应用完整功能",
            FontSize = 12,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Foreground = Brushes.Gray,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });

        _contentBorder.Child = fallbackContent;
        UpdateStatus("回退到基础模式", Brushes.Orange);
    }

    private void UpdateStatus(string message, IBrush color)
    {
        if (_statusText != null)
        {
            _statusText.Text = message;
            _statusText.Foreground = color;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            UpdateSource();
        }
    }

    private void UpdateSource()
    {
        _currentUrl = Source ?? "";
        
        bool isValidUrl = !string.IsNullOrEmpty(_currentUrl) && 
                         _currentUrl != "未启动" && 
                         Uri.IsWellFormedUriString(_currentUrl, UriKind.Absolute);
        
        if (_refreshButton != null)
        {
            _refreshButton.IsEnabled = isValidUrl;
        }

        if (_openExternalButton != null)
        {
            _openExternalButton.IsEnabled = isValidUrl;
        }

        if (isValidUrl && _isWebViewReady)
        {
            _ = NavigateToUrlAsync(_currentUrl);
        }
        else if (string.IsNullOrEmpty(_currentUrl) || _currentUrl == "未启动")
        {
            UpdateStatus("等待 NCF 启动", Brushes.Gray);
        }
    }

    private async Task NavigateToUrlAsync(string url)
    {
        try
        {
            UpdateStatus($"正在连接到: {url}", Brushes.Blue);
            OnNavigationStarted(url);

#if WINDOWS
            if (_isWebView2Ready && _webView2?.CoreWebView2 != null)
            {
                await NavigateWebView2(url);
                return;
            }
#endif

            // 非Windows平台或WebView2不可用时的处理
            var currentContent = _contentBorder.Child;
            if (currentContent is ScrollViewer scrollViewer && 
                scrollViewer.Content is StackPanel stackPanel)
            {
                // 清除之前的内容，保留标题
                while (stackPanel.Children.Count > 2)
                {
                    stackPanel.Children.RemoveAt(stackPanel.Children.Count - 1);
                }
                
                await FetchPageInfoAsync(url, stackPanel);
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"导航失败: {ex.Message}", Brushes.Red);
            OnNavigationFailed(ex.Message);
        }
    }

    public async Task NavigateTo(string url)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Source = url;
        });
    }

    private void OnRefreshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_currentUrl))
        {
            _ = NavigateToUrlAsync(_currentUrl);
        }
    }

    private void OnOpenExternalClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_currentUrl))
        {
            OpenInExternalBrowser(_currentUrl);
        }
    }

    private void OpenInExternalBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }

            UpdateStatus("已在外部浏览器中打开", Brushes.Blue);
        }
        catch (Exception ex)
        {
            UpdateStatus($"无法打开外部浏览器: {ex.Message}", Brushes.Red);
            OnNavigationFailed($"无法打开外部浏览器: {ex.Message}");
        }
    }

    // 事件定义
    public event EventHandler<string>? NavigationStarted;
    public event EventHandler<string>? NavigationCompleted;
    public event EventHandler<string>? NavigationFailed;

    protected virtual void OnNavigationStarted(string url)
    {
        NavigationStarted?.Invoke(this, url);
    }

    protected virtual void OnNavigationCompleted(string url)
    {
        NavigationCompleted?.Invoke(this, url);
    }

    protected virtual void OnNavigationFailed(string error)
    {
        NavigationFailed?.Invoke(this, error);
    }
} 