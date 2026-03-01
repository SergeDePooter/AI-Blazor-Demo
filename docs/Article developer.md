# Can we still be developers?

It is now already commonly known that AI is here to stay. In what form, we cannot say but what we can say, is that the role of the developer is changing drastically and more important, it is changing fast. I must admit that until the summer of 2025, I was one on the skeptical side on the use and progress of AI. When I used it, I often wasn't satisfied with the results. AI didn't seem to understand me, and I didn't understand AI. However, when newer and better models where deployed in the fall of 2025, my view changed entirely. Here is my story.

## The hobby project
All developers know it: you start a hobby project, you're enthusiastic, you make a repository, start developing and then ... life happens. Working full-time, having a wife and kids, other hobbies and suddenly, the hobby project feels like an item on your ToDo-list. Because hey, apparently you still have to sleep as well to function as a person. 

For me, it all started with a simple static webapp, that should be able to track statistics for a sport club. It was mainly UI, plain HTML and some connection to an Azure SQL Database. The concept was simple, as well as the data but I just didn't feel like it, to plunge into the HTML and map everything with the data and vice versa. 

Enter Claude. In no-time, the full website was created in HTML, I just needed to do some wiring to connect the data with the UI and done. Nothing more to it, everything I needed was requested and provided by an AI agent, working like it should. It was the first realization that maybe this AI trend wasn't just some tooling.

## The insight (working title)
A little fast forwarding to january of this year. Between fall and january, I often used AI  in my day-to-day job, asking for code generation or UI outlines. It became more than a tool, it felt like a sparring partner. For most of the time I was pleased with the outcome, which made me curious about how far this could go. And especially, how could we use it to make something 'out-of-the-box'. 

At Xebia, we have these days called 'Innovation days', where we get to work / investigate / experiment with (new) technology for a full day. In the most recent day, in january, I had doubts about the topic I would choose (because some colleagues had interesting topics as well). Finally, I went for a full day exploring AI, using different models and using SpecKit. 

One of the topics I wanted to investigate is how AI deals with architectural questions and assignments. All new projects start with an architectural setup in a solution, wether you create a backend, frontend, service, ... In a real life scenario, where you want to keep control of your solution, this was one of my essential questions still open in the usage of AI. During the day, I used different agents like Claude Code and Github Copilot but in the end, they all failed (dramatically) in my basic questioning. None of the agents was able to give a satisfactory results. 

Here is what I asked them:
```
Create me a solution, called SomeSolution. I want this solution to following an architectural setup of vertical-slicing architecture, using CQRS, with Blazor as frontend.
``` 
Different agents, different outcomes:
- A blazor project with CQRS within the blazor project itself.
- A blazor project with CQRS, in a different project but with direct interface integration within the blazor project
- A blazor project, without CQRS, but with an API, within the blazor project itself
- A blazor project with vertical slicing within the web application but nothing more. 
- ...

It seemed like it had little to no notion on how to setup something like this, and couldn't figure out the puzzle with all the separate components as well. My best guess is that this might not be so surprising as we think it is. Agents are mainly build on code but do we expect that a source, like for instance github, is full of well architected projects?

## The demo project
As you can see, for me AI failed in setting up architectural question. Yes, it could answer them like a textbook (and sometimes also gave insights) but it couldn't integrate the textbook in a solution setup. Time for a shift in focus. What if I outline my architecture in a solution and then start working with AI? It is a different approach but it felt more to reality where we often work with existing solution architectures or need to come up with one ourselves.

Here is my outline of my demo project (a citytrip planner for exploring and creation):
  ```
  CitytripPlanner/
  └── CitytripPlanner.Features/
      ├── CityTrips/
        ├── CreateTrip/
        │   ├── CreateTripCommand.cs
        │   ├── CreateTripHandler.cs
        │   └── CreateTripValidator.cs
      ├── UserProfiles/
        ...

├── CitytripPlanner.Infrastructure/
...
├── CitytripPlanner.Web/
...
  ```
It is feature based, it uses CQRS and it has a neath separation of concerns regarding dependencies. Plain simple, a not too fancy setup but efficient. After all, it still is a demo project.

For my features, I relied on the usage of speckit (https://speckit.org/). It is a useful tool when starting exploring AI or if you don't have too much experience with coding or breaking down requirements in comprehendible (sub)tasks. Not going into details here, but make sure to check it out. 
```
On a side note here: in my opinion, when you use speckit a couple of times, speckit makes itself obsolete because you can learn yourself to anticipate most steps it uses.
```

And there it went. I requested feature after feature, following a TDD approach, following the rules of vertical slicing architecture, using CQRS. I didn't have to think about my architecture, my only concern was what to ask next. But yes, there a several remarks to make.

What is not working well:
1. I didn't connect my application to a real database. Everything is in a in-memory database. Great for demo-purpose, not so great if you really want to use it.
2. Related to point 1, is scalability. I didn't bother asking question or requirements regarding scalability and it shows. My DBContext is inject directly everywhere, which wouldn't stand long in a real-time application, if it is used widely.
3. Following a TDD approach is all good, but many tests are only testing the 'happy-path'. It are some tests (better than nothing) but for real development scenario's, this doesn't suffice.
4. The code is not (all) state-of-the-art but it often gives direction or basic to work with.

The good news about the things that aren't working well: if we are clear enough against AI, I'm sure we can delegate tasks to fix these

## From demo to development
Now came the real test: is this ready to be used in a real development cycle? The outline of the demo project was quite boiler-plate and not a legacy project with different structure or approach. My solution only exists with a couple of projects, what if the solution is a lot bigger / complexer? Well, only 1 way to find out.

I had the opportunity to pitch this idea, with thanks to my own demo project, to my customer and to convince them to, at least, setup a proof-of-concept with this. As the code is specific to my customer, I cannot go in to detail there but I can give the used approach for the proof-of-concept. I setup Claude Code as AI agent and let it make a claude.md file using the existing project with its documentation as source. And yes, the project was also CQRS and vertical slicing.

As a consultant, we are being paid to do our work, within the given timeframe and to deliver high quality code / solutions. The current project was under pressure to deliver within the next few months so there was not too much room for lacking on deliverables. 

My solution: I pulled the git-repository in a new folder, which would only be used for the POC. This way, I wouldn't be blocked with my own work and an AI instance could take up new user story while I was working as well. If the changes it did weren't good, I could just discard them and take up the story myself. 

This is the point where my mind was blown away. In the first few days, I was able to delegate tasks (user stories, refactors, catching up on some testing ...) to the AI instance and in most cases, it just delivered what I wanted for at least 70 to 80 percent (sometimes even more). I still had to do some work but mostly it was just the last bits and pieces to make everything work like it should. Even tasks which were estimated a full days work (a team of 5 senior developers estimated the hours), were done in a hour or less. It wasn't just vibe-coding, it was like there was an extra developer in our team, working faster and often more consistent than any of us.

But truth must be told, it wasn't all good. There were moments and generated code I just discarded and either tried again with AI or took up myself. For instance, one of the things I threw out, was related to a new service setup. The AI had, once again, difficulties on solving some architectural questions. In other stories, it seemed that some questions weren't outlined enough or didn't have the proper context to handle the questions. I guess this are the lessons learned: how to proper prepare requirements and AI agents to work the best so that we don't have these kind of issues. As the joke goes: it starts with clearly defining on what you want as a developer /  business.

## The real question
With all the information and lessons learned from above, it rises the real question:
```
Can we still be developers?
```
Looking at the pace of the new releases of AI, it makes me wonder it we still need a developer in the definition of what a developer is today: someone who writes code day-in, day-out, ploughing through all the user stories and bugs ... 

The answer to the question has two side to it. 
1. We need to make sure we are in the drivers seat. If we can outline our own vision and do our (clean) setup, AI can be our new colleague on the virtual floor. And the approaches can be a variety of options: use it as a guardian on code quality, use it as a senior developer for code reviewing. Or like I tried to do: use it as a senior developer.

2. The definition of a developer is shifting. We will not be the day-in, day-out developer anymore that we used to be. We will become more something like what I like to call: an input - validator. We will need to use our knowledge to generate decent input for the AI and we need to validate the outcome of what AI did, as long as we work with code that we want to understand.

So my key conclusion: the 'old' developer will need to adapt to the new reality, where in my opinion, the solution - architectural question will become more and more important. And it might not stop there: maybe the developer should not only transform to a solution architect but even further (enterprise architecture). 

This being said: how long do you think the new role will hold?

