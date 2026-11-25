namespace OBD.Mobile;

/// <summary>
/// The UIScene lifecycle of the app on iOS. An app built with the iOS 27 SDK that does not adopt it never gets past
/// its launch screen on iOS 27: UIKit stops in its check for the scene lifecycle
/// (<c>_UIApplicationEvaluateRuntimeIssueForNoSceneLifecycleAdoption</c>) and the system kills the app, which Apple's
/// review reports as a crash on launch. MAUI does everything itself: the app only declares this delegate in its
/// Info.plist, under <c>UIApplicationSceneManifest</c>, with the configuration MAUI asks for:
/// <code>
/// &lt;key&gt;UIApplicationSceneManifest&lt;/key&gt;
/// &lt;dict&gt;
///   &lt;key&gt;UIApplicationSupportsMultipleScenes&lt;/key&gt;&lt;false/&gt;
///   &lt;key&gt;UISceneConfigurations&lt;/key&gt;
///   &lt;dict&gt;
///     &lt;key&gt;UIWindowSceneSessionRoleApplication&lt;/key&gt;
///     &lt;array&gt;
///       &lt;dict&gt;
///         &lt;key&gt;UISceneConfigurationName&lt;/key&gt;&lt;string&gt;__MAUI_DEFAULT_SCENE_CONFIGURATION__&lt;/string&gt;
///         &lt;key&gt;UISceneDelegateClassName&lt;/key&gt;&lt;string&gt;SceneDelegate&lt;/string&gt;
///       &lt;/dict&gt;
///     &lt;/array&gt;
///   &lt;/dict&gt;
/// &lt;/dict&gt;
/// </code>
/// Scenes exist since iOS 13, below the app's minimum: older versions run the same lifecycle.
/// </summary>
[Foundation.Register("SceneDelegate")]
public class SceneDelegate : Microsoft.Maui.MauiUISceneDelegate
{
}
