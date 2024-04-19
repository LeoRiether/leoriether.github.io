+++
title = "Keeping my bookmarks in a plaintext file"
date = 2024-04-17
[taxonomies]
categories = ["practices"]
+++

For the longest time I didn't organize my bookmarks at all. Links were either in
an unorganized folder with various miscellaneous neighbors or inside a "Imported
from Firefox" folder nested inside "Imported from Chrome" (repeat however many
times I changed browsers in my life).

Worse than that, because I didn't get much use out of bookmarks I never got into
the habit of bookmarking things a lot, which reinforces the whole not getting
much use out of them.

At some point, however, I found [vimwiki](https://github.com/vimwiki/vimwiki)
and got more invested in the way I take notes. I've since moved to [mkdnflow](https://github.com/jakewvincent/mkdnflow.nvim),
but these tools serve the same purpose: they allow me to keep a local, easily
accessible knowledge base that's searchable, easy to backup, stored in a very
commonly accepted format! Plus, I can bind a key to open it on `$EDITOR`.

But the purpose of this post isn't to explain note-taking systems in general,
you can Google "[Zettelkasten](https://en.wikipedia.org/wiki/Zettelkasten)" or watch [Hack your brain with Obsidian.md](https://www.youtube.com/watch?v=DbsAQSIKQXk)
for that, maybe even [Hack Your Brain With Elaborate Coping Mechanisms](https://www.youtube.com/watch?v=XUZ9VATeF_4)
while you're at it -- it's about more than just notes, but it'll be worth it I
promise. The purpose of this post is to say why I now keep my bookmarks in said
note-taking system, in a list inside a markdown file.

Sure, the first thing I said is that "I didn't organize my bookmarks" and while
throwing every article or youtube video I have a slight chance of wanting to
revisit later isn't exactly organized, a list of links in chronological order
that I can easily Ctrl+F if needed actually works quite well! All the other
benefits of a local file that's not inside a browser apply here too, making it
easy to backup and avoding the need to export/import them. Keybinds help in
making it easy to open the file to add a new link, but obviously pressing Ctrl+D
is easier, so I'll at least concede that point to the browsers.

If I do regret choosing this format one day, markdown is just so simple I could
hack a quick script to convert it to another format and call it a day. Will I
regret it? Only time will tell. Maybe I'll edit this post 2 years from now and
report back.

