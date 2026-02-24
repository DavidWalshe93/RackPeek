# Tasks: Add Key-Value Labels to Resources

## 1. Domain Model and Validation

- [x] 1.1 Add `Labels` property (`Dictionary<string, string>`) to `Resource` base class, initialized to `new()`
- [x] 1.2 Add `Normalize.LabelKey` and `Normalize.LabelValue` helpers (trim)
- [x] 1.3 Add `ThrowIfInvalid.LabelKey` and `ThrowIfInvalid.LabelValue` (non-empty, key ≤ 50 chars, value ≤ 200 chars)
- [x] 1.4 Create `AddLabelUseCase<T>` and `IAddLabelUseCase<T>` in `UseCases/Labels/`
- [x] 1.5 Create `RemoveLabelUseCase<T>` and `IRemoveLabelUseCase<T>` in `UseCases/Labels/`
- [x] 1.6 Register `IAddLabelUseCase<>` and `IRemoveLabelUseCase<>` in `ServiceCollectionExtensions`

## 2. Persistence

- [x] 2.1 Verify YamlDotNet serializes/deserializes `Dictionary<string, string>` for labels (no custom converter)
- [x] 2.2 Add unit test for YAML round-trip with labels and legacy YAML without labels

## 3. CLI Commands

- [x] 3.1 Create `ServerLabelAddCommand` and `ServerLabelRemoveCommand` with `--key` and `--value` options
- [x] 3.2 Add `label` branch with `add` and `remove` commands to servers in CliBootstrap
- [x] 3.3 Add label add/remove commands for switches, routers, firewalls
- [x] 3.4 Add label add/remove commands for systems, accesspoints, ups
- [x] 3.5 Add label add/remove commands for desktops, laptops, services
- [x] 3.6 Update `ServerDescribeCommand` to display labels in key-value format
- [x] 3.7 Update describe commands for switches, routers, firewalls, systems, accesspoints, ups, desktops, laptops, services to display labels

## 4. Web UI

- [x] 4.1 Create `ResourceLabelEditor` component in Shared.Rcl (mirror `ResourceTagEditor`, use `KeyValueModal`)
- [x] 4.2 Add `ResourceLabelEditor` to ServerCardComponent
- [x] 4.3 Add `ResourceLabelEditor` to SwitchCardComponent, RouterCardComponent, FirewallCardComponent
- [x] 4.4 Add `ResourceLabelEditor` to SystemCardComponent, AccessPointCardComponent, UpsCardComponent
- [x] 4.5 Add `ResourceLabelEditor` to DesktopCardComponent, LaptopCardComponent, ServiceCardComponent

## 5. Unit Tests

- [x] 5.1 Add `AddLabelUseCaseTests` (new label, overwrite existing key, nonexistent resource)
- [x] 5.2 Add `RemoveLabelUseCaseTests` (remove existing, remove nonexistent key no-op)
- [x] 5.3 Add validation tests for `ThrowIfInvalid.LabelKey` and `ThrowIfInvalid.LabelValue`

## 6. E2E Tests

- [x] 6.1 Add CLI workflow test: add label, verify YAML, describe shows labels, remove label
- [x] 6.2 Add browser E2E test for label add/remove in resource card
