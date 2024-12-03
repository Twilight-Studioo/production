#!/bin/bash

# 引数として major, minor, patch のいずれかを受け取る
VERSION_TYPE=$1

# package.json を更新
npm version $VERSION_TYPE --no-git-tag-version

NEW_VERSION=$(node -p "require('./package.json').version")
# Git でコミットとタグを追加
git add package.json
git commit -m "Release version ${NEW_VERSION}"
git tag "v${NEW_VERSION}"