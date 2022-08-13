# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- PersistenceSettings.OnSaveError
- PersistenceSettings.OnLoadError
- LoadingUIHandler component

### Changed
- Refactor PersistenceSettings.Save into a Task function
- Replace PersistenceSettings.TryLoad function into a Task Load function

### Removed
- FileSystem.SaveUncompressed function
- PersistenceSettings.TryLoad function

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

[Unreleased]: https://github.com/HyagoOliveira/Persistence/compare/1.0.0...main
[1.0.0]: https://github.com/HyagoOliveira/Persistence/tree/1.0.0/