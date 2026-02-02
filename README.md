🧠 Project Overview

This project implements a Smart Traffic Controller responsible for coordinating vehicle signals, pedestrian crossings, and timing logic for a smart traffic light system.

The solution is developed using a Test-Driven Development (TDD) approach, ensuring all functionality is specified, verified, and validated through automated unit tests.

⸻

🏗️ Architecture Overview

🔹 Core Class

TrafficController
Acts as the central coordinator and is responsible for:
	•	🚗 Vehicle signal control
	•	🚶 Pedestrian signal management
	•	⏱️ Timing coordination
	•	🌐 Logging signal changes via a web service
	•	✉️ Sending maintenance notifications via email

⸻

🔹 Dependency Interfaces

To ensure loose coupling and test isolation, the following interfaces are used:
	•	IVehicleSignalManager
	•	IPedestrianSignalManager
	•	ITimeManager
	•	IWebService
	•	IEmailService

⸻

🧪 Testing Strategy
	•	Unit tests implemented in TrafficControllerTests.cs
	•	Follows Arrange / Act / Assert
	•	Each test maps to a specific requirement (Appendix A)
	•	Parameterised tests using [TestCase]
	•	Dependencies mocked using NSubstitute
	•	Focus on correctness, edge cases, and behaviour validation

⸻

🔁 TDD Workflow

1️⃣ Write a failing test
2️⃣ Implement minimal logic
3️⃣ Refactor safely
4️⃣ Repeat per requirement

⸻


