# Contribution guidelines

This document is just a guideline for anyone looking to contribute to this repository, which I greatly appreciate.

These guidelines aim to maintain a consistent and sustainable style and structure for the project.

## Committing changes

### Getting started

To start contributing, fork this repository and make changes within your fork.
Once you are finished, you can create a pull request that targets the main branch of this original repository.

Your pull request will be reviewed, and if everything is fine, it'll be merged.

### How to commit

Please split your work into smaller commits.
This makes reading the history and potentially rolling back much easier. Do not create a giant commit and call it *Did stuff*.

Sometimes we can get into the zone and make changes across a ton of files. Split those changes using line staging. Try to ensure that every isolated commit is buildable (even though it isn't always possible).

### Commit messages

Commit messages should be prefixed with `#`, followed by the issue number, then a dash, and finally the commit message in imperative form.

Let's say you are working on issue number 42, which is about adding a new UI component to the project. Here are a couple of commits that could potentially be created when solving this issue:

- `#42 - Prepare UI component file`
- `#42 - Add required buttons`
- `#42 - Adjust styling`
- `#42 - Wire-in the View Model`
- `#42 - Fix typo`

## Code style

This repository contains an `.editorconfig` which should push you towards the expected coding style via warnings.

### Fundamentals

The key points are:
- Use spaces that are 4 characters wide
- `const` and `static` fields are upper camel case
- Non-static fields should be prefixed with `_`
- Single-line methods should use the expression format

Ensure that you postfix your asynchronous method names with `Async`. E.g., `ReadDataAsync`.

### Complexity

Avoid producing cognitively complex code:
- Split your complicated long methods into multiple reusable methods.
- Do not use `goto`. Each time you use it, Vagner strangles a cat (*CTU FIT reference*).
- Don't repeat yourself is a rule that can be broken - balance out writing complex, generic code with an occasional duplication.
- Don't preemptively optimize the code.
- Use LINQ if performance is not crucial.

## Documentation

Documentation is a crucial component of any project. There is no single way to document a project that negates others; thus, this project utilizes the following principles:

- Code documentation comments
  - `public` members must be documented using the XML documentation format. Use the appropriate `summary`, `remarks`, `param`, `paramref`, `returns`, etc. tags to add as much information as possible.
  - Complex algorithms should be documented line by line. Use classic `//` comments to describe what each line is doing in the given algorithm.
  - When you change something, **update the comments** - this is Open Source, not a corporate project with tight deadlines.
- Descriptive naming
  - Applies to all members and variables. Avoid `ctx`, use `context`. Avoid `var rwls = ReadWriteLockSlim()`, use `var lock = ReadWriteLockSlim()` or `lockRW`.
  - This is a difficult thing to balance; however, the better the naming, the more readable code you produce
- User/Dev documentation
  - This repository has a Wiki that must reflect the current state of the project. Please update it if you make changes affecting the UX/DX.
- Unit tests (TBD)
