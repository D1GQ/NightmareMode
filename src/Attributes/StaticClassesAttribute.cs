using NightmareMode.Interfaces;
using System.Reflection;

namespace NightmareMode.Attributes;

/// <summary>
/// Base abstract class for custom attributes that automatically register instances of types
/// marked with specific attributes. Provides automatic discovery and registration system
/// for game content like nights and challenges.
/// </summary>
internal abstract class InstanceAttribute : Attribute
{
    /// <summary>
    /// Scans the current assembly for all sealed subclasses of InstanceAttribute,
    /// creates instances of them, and triggers their registration process.
    /// Called during mod initialization to set up all registered content.
    /// </summary>
    internal static void RegisterAll()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var types = assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(InstanceAttribute)) && !t.IsAbstract && t.IsSealed)
            .ToArray();

        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is InstanceAttribute attribute)
            {
                attribute.RegisterInstances();
            }
        }
    }

    /// <summary>
    /// When overridden in a derived class, handles the registration of all types
    /// marked with this attribute.
    /// </summary>
    protected abstract void RegisterInstances();
}

/// <summary>
/// Generic abstract base class for attributes that register static instances of a specific type.
/// Provides collection management and lookup functionality for registered instances.
/// </summary>
/// <typeparam name="T">The type of objects being registered (must be a class).</typeparam>
[AttributeUsage(AttributeTargets.Class)]
internal abstract class StaticInstanceAttribute<T> : InstanceAttribute where T : class
{
    private static readonly List<T> _instances = [];

    /// <summary>
    /// Gets a read-only list of all registered instances of type T.
    /// </summary>
    internal static IReadOnlyList<T> Instances => _instances.AsReadOnly();

    /// <summary>
    /// Gets a registered instance of the specified type J.
    /// </summary>
    /// <typeparam name="J">The specific type to look up (must be assignable to T).</typeparam>
    /// <returns>The first instance of type J found, or null if none exists.</returns>
    internal static J? GetClassInstance<J>() where J : class =>
        _instances.FirstOrDefault(instance => instance.GetType() == typeof(J)) as J;

    /// <summary>
    /// Gets a registered instance that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each instance for a condition.</param>
    /// <returns>The first instance that matches the predicate, or null if none found.</returns>
    internal static T? GetClassInstance(Func<T, bool> predicate) =>
        _instances.FirstOrDefault(predicate);

    /// <summary>
    /// Scans the assembly for all types that have this attribute, creates instances of them,
    /// and adds them to the static instances collection.
    /// </summary>
    protected override void RegisterInstances()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var attributedTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(GetType(), false).Any());

        foreach (var type in attributedTypes)
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
                if (constructor != null && constructor.Invoke(null) is T instance)
                {
                    _instances.Add(instance);
                }
            }
        }
    }
}

/// <summary>
/// Attribute for registering night instances in the game.
/// Types marked with this attribute will be automatically discovered and
/// registered as available nights.
/// </summary>
internal sealed class RegisterNightAttribute : StaticInstanceAttribute<INight>
{
}

/// <summary>
/// Attribute for registering challenge instances in the game.
/// Types marked with this attribute will be automatically discovered and
/// registered as available challenges.
/// </summary>
internal sealed class RegisterChallengeAttribute : StaticInstanceAttribute<IChallenge>
{
}