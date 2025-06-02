# Agents Todo Application - Semantic Kernel

This application assists users in creating Todo task lists for things they want to achieve.

The goal is to move beyond single-user, stateful, console applications, and towards the types of applications .NET developers currently build in organizational and production environments.

## Agent Orchestration

* The design started with a single agent creating Todo tasks based on user input/descriptions.
* An Orchestrator was then introduced, which selects and routes requests to a generic worker agent (a single, general-purpose Todo task agent).
* A specific Travel Agent was added with prompts tailored to holiday planning. When the user wants to plan a trip, the Orchestrator selects this agent.
* Any specific type of planning agent can now be added via configuration to the application, and the Orchestrator will choose based on the context.

## Technical Design

* The application is stateless (Scoped DI) and loads conversation history, tasks, etc., on each request.
* Application Insights Telemetry is used to track agent and user interactions.
* Agent work is managed using a loose implementation of the Google A2A protocol. It uses ASP.NET (not JSON-RPC) to accept input from the user and SignalR to return responses.
* Agent tasks and conversation history are stored and loaded from Azure Storage (Blob).
* Orchestrator-to-worker agent communication is decoupled using MediatR.
* Agents are constructed from settings files and use middleware to extend their behavior.

## Roadmap

* React frontend with Azure B2C
* Persist user Todo lists on Azure Storage
* RAG with Azure AI Search
* Orchestrator communication with an externally hosted agent


