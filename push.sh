#!/usr/bin/bash

echo -n "You want to push [file/.]: "
read input
git init
git add $input
git status
echo -n "Enter commit message: "
read msg
git commit -m "$msg"
echo -n "push to [branchName]: "
read branch
git push origin $branch