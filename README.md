# Ember Toolkit

Ember Toolkit is a comprehensive suite designed to enhance Unity game development, focusing on data management, serialization, and integration with Unity Services. It provides a robust framework for managing game states, saveable objects, and encrypted data serialization, ensuring a seamless and secure gaming experience.

## Features

- **Data Management**: Centralized handling of game data, including saving and loading game states and objects.
- **Serialization**: Utilizes a custom JSON serializer for efficient data serialization, with optional AES encryption for added security.
- **Dependency Injection**: Leverages Microsoft's Dependency Injection framework to manage services and configurations, promoting a modular and testable codebase.
- **Unity Services Integration**: Simplifies the integration with Unity Services, providing a streamlined workflow for accessing and utilizing Unity's extensive service offerings.
- **Flexible Configuration**: Supports custom settings through the `IEmberSettings` interface, allowing for easy adaptation to specific project requirements.

## Getting Started

To integrate Ember Toolkit into your Unity project, follow these steps:

1. **Add the Package**: Clone or download the Ember Toolkit package into your Unity project's `Packages` directory.
2. **Configure Services**: Utilize the `Configurator` class to register your dependencies and services in the `IServiceCollection`. This includes setting up the `IEmberSettings` implementation and any required services like `IAESController` for encryption.
3. **Initialization**: In your game's initialization phase, create an instance of `EmberServiceConnector` with your `IEmberSettings` implementation to set up the dependency injection container and initialize all services.
4. **Usage**: Use the provided `EmberBehaviour` and `GameState` classes as bases for your game's behaviors and states. Utilize the `RequestService<T>` method to access services managed by Ember Toolkit.

## Example



## Documentation

For detailed documentation on each component and service, refer to the inline comments within each class and interface. The toolkit is designed to be self-explanatory, with clear naming conventions and comprehensive comments guiding you through its usage.

## Contributing

Contributions to Ember Toolkit are welcome! Whether it's through submitting bug reports, feature requests, or pull requests, your input is valuable in making this toolkit more robust and versatile.

## License

Ember Toolkit is licensed under [MIT License](LICENSE.md). Feel free to use, modify, and distribute it as part of your projects.
