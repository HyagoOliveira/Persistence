# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- PersistenceSettings.GetLastSlot function

### Fixed
- FileSystem.OpenSaveFolder

### Changed
- Files are saved/loaded inside Persistence folder

## [3.0.0] - 2024-11-27
### Added
- PersistenceSettings.TryDelete function
- PersistenceSettings.TryDeleteAll function
- PersistenceSettings.GetNames function
- Unity Json Serialization backend
- Support to ScriptableObject serialization

### Changed
- Rename and refactor function PersistenceSettings.Load -> PersistenceSettings.TryLoad
- Add optional useRawFile parameter into PersistenceSettings.Load functions

### Removed
- IPersistenceSettings interface
- IFileSystem interface
- Static PersistenceSettings.LoadRawFile function

## [2.2.0] - 2023-03-28
### Added
- Fast load raw file
- Check if generic type is serializable before load/save

### Fixed
- Add missing CreateAssetMenu attribute in PersistenceSettings
- Add missing namespace for NewtonsoftSerializer
- Flushes the data changes into the Browser IndexedDB when saved

## [2.1.1] - 2023-03-15
### Fixed
- Disable UIHandler UI GameObjects even if TimeScale is set to 0

## [2.1.0] - 2023-03-14
### Added
- Newtonsoft Json Serializer

### Changed
- Remove CreateAssetMenu attribute from PersistenceSettings
- Using BinaryFormatter only if BINARY_AVAILABLE
- Pretty serialize raw file data

## [2.0.0] - 2022-08-13
### Added
- Asyn-IO package 1.0.0
- PersistenceSettings.OnSaveError
- PersistenceSettings.OnLoadError
- LoadingUIHandler component
- SavingUIHandler component
- Cryptography package 1.0.0

### Changed
- Refactor PersistenceSettings.Save into a Task function
- Replace PersistenceSettings.TryLoad function into a Task Load function
- Replace local Cryptographers classes for Cryptography package

### Removed
- FileSystem.SaveUncompressed function
- PersistenceSettings.TryLoad function
- Cryptographers classes

## [1.0.0] - 2022-06-29
### Added
- Add Persistence Settings Provider
- Persistence Settings
- File System class
- Xml Serializer
- Json Utility Serializer
- Binary Serializer
- AES Cryptographer
- GZip Compressor
- Scriptable Settings Provider package dependency
- CHANGELOG
- Package file
- README
- LICENSE
- gitignore
- Initial commit

[Unreleased]: https://github.com/HyagoOliveira/Persistence/compare/3.0.0...main
[3.0.0]: https://github.com/HyagoOliveira/Persistence/tree/3.0.0/
[2.2.0]: https://github.com/HyagoOliveira/Persistence/tree/2.2.0/
[2.1.1]: https://github.com/HyagoOliveira/Persistence/tree/2.1.1/
[2.1.0]: https://github.com/HyagoOliveira/Persistence/tree/2.1.0/
[2.0.0]: https://github.com/HyagoOliveira/Persistence/tree/2.0.0/
[1.0.0]: https://github.com/HyagoOliveira/Persistence/tree/1.0.0/