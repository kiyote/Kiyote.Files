// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage( "Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names are METHOD_CONDITION_EXPECTATION", Scope = "module" )]
[assembly: SuppressMessage( "Design", "CA1034:Nested types should not be visible", Justification = "Makes for nicer referencing", Scope = "type", Target = "~T:Kiyote.Files.Virtual.IntegrationTests.FS.Test" )]
[assembly: SuppressMessage( "Style", "IDE0130:Namespace does not match folder structure", Justification = "Testing namespace has test last in its name.", Scope = "module" )]
[assembly: SuppressMessage( "Maintainability", "CA1515:Consider making public types internal", Justification = "Code not released, doesn't mattter.", Scope = "module" )]
