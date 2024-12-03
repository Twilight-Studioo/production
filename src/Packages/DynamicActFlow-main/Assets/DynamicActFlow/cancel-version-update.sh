#!/bin/bash

# 最新のコミットを取り消し
git reset --hard HEAD~1

# 最新のタグを削除
LATEST_TAG=$(git describe --tags `git rev-list --tags --max-count=1`)
git tag -d $LATEST_TAG