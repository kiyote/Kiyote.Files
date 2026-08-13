# Developer Notes

Suggestions for improving documentation are welcome.

### Notes

Github action failing with permission denied?
```
git update-index --chmod=+x ./create-orphan-branch.sh
```

Still failing?  Try running the `./create-orphan-branch.sh` locally once.


Coverage badge failing to be pushed?

Give the workflow the `contents: write` permission.

