# Agent Instructions & Guidelines

## 1. Git Workflow & Commit Rules
- **Explicit Request Only**: NEVER run `git commit` or `git push` unless explicitly asked by the user (e.g., when the user types `commit`, `push`, or asks to save to git).
- **Naming Convention**: When committing, always adhere to the **Conventional Commits** specification (e.g., `feat:`, `fix:`, `refactor:`, `style:`, `test:`, `chore:`, `docs:`).
- **Language**: Always write commit messages and documentation in **English**.

## 2. Project Architecture & Coding Standards
- **Framework**: .NET 8 Web API.
- **C# Standards**: Use modern C# 12 features where appropriate (e.g., Primary Constructors, collection expressions).
- **Environment Configuration**: Sensitive settings (database connection strings, JWT keys/secrets) must be read strictly from environment variables (`.env`) without hardcoded secrets or fallbacks in `appsettings.json`.
- **Patterns**:
  - **Fluent API**: Table mapping, relationships, and constraints in dedicated `IEntityTypeConfiguration<T>` classes.
  - **FluentValidation**: Request validations in `AbstractValidator<T>` classes returning unified `ServiceResult<T>` error structure.
  - **Repository & Unit of Work**: Data access abstraction via repositories and `IUnitOfWork`.
  - **DTO & AutoMapper**: Strictly decouple domain entities from API contracts using DTOs and AutoMapper profiles.
  - **ServiceResult<T>**: Consistent response envelope for all service operations and API responses.

