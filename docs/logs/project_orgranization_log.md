# Project organization
Put all logs related to how we set up our project structure and Github project here. Planning is laying out 
what to do. Scheduling is estimating effort and deadlines for tasks made in planning.

<!-- todo(turnipxenon): Break apart to planning, scheduling and project organization -->

**[2021-10-12]**
# The Beginning: Planning 👀

I don't really know what I'm doing, but I want to make this project structured
and transparent. So, I'm writing this log of my current thoughts as I go, and
see how it evolves over time.

I did not get to document my earlier attempts but from what I remember it was
something like:
- Okay, let's make Github Projects! Let's automate it and hope that it works
as intended. I haven't really cleaned up Github Projects, and I think I need
to read more about how other people use it for.
- Signed up for the Beta program for Github Projects, hope we get in. Putting
the date here to see how many days I'll be waiting to get in. *2021-10-12*

So, where am I at? I'm currently making pull request templates. I now have the
pull request template, I just have to document how to use the extra templates
for myself in the future if we ever need it.

**[2021-10-13]**
Issue template time!

We have these two helpful links:
- https://www.backhub.co/blog/best-practices-for-using-github-issues
- https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests/configuring-issue-templates-for-your-repository

Issue templates seem straightforward. I think I should not be thinking about
how issues should look like since this project is not open to the community, yet.
Issues are fine to be a few liners, and keep the details in the PRs they are 
attached to.

...

I would like to use milestones to learn planning and how much work I can do. The
question I have right now is... Where do I write my documentation for my
investigatory work?

I also would like to note the workflow locally. I just realized I would often...
push things to main accidentally. I locked main, and restricted changes to only be
made via pull request.

...

I've decided to put the documentation about the design, scope and specifications
under `docs/projects`. We're calling this one `PROJ_0001_prototype.md`. I'm debating
whether the small scale feature specs should be inside a folder next to the project containing
it or should it be in a separate folder from `projects`. On second thought, we may
end up with a huge folder filled with features, so maybe it's best to put it next to
the project it belongs to... Which leads me to the other question: which project
should a feature belong to? The one it was concepted with? The one where work for it started in?
The one it finished in? My gut feeling here would be the one where work for it started
in but I may make exceptions when the majority of the work is distanced from the
time it began.

**[2021-10-17]**
I got into beta around the 14th, but... I can't migrate my projects to beta,
so I have to do it all manually. My current plan today is to migrate from our repository,
to our personal project board. Then, we can continue our investigatory work and
writing for our current project.

**[2021-10-18]**
Just finished migrating to Projects Beta and it feels laggy. I really like the more condensed table view,
but I also like kanban boards which will be helpful for other uses. If only they had organization-wide
milestones instead of having them restricted to a repository...

Not all the keyboards and actions are up yet. Some are hidden in the documentation. I guess it really is
beta 🤣. So, note to self: # to make new item in the kanban board.

Update! They have a yet-to-be-documented feature called [iterations](https://github.blog/changelog/2021-10-14-the-new-github-issues-10-14-update/)
which is basically just sprints! That's what I need. Now, I can instead repurpose Github milestones
as a way to... well break up milestones! This is cool! These changes a lot of things.
I can use milestones to be what used to be projects. They would encapsulate versions or chunks of the product
that I want. Projects can now host all of my milestones with the very useful views. Okay, I think I got
in Github Projects in a very nice time.