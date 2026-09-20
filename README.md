Bounce Dash
An indie 3D mobile platformer focused on the full development cycle.

(Insert a short, eye-catching GIF of your gameplay here)

📌 Overview
Bounce Dash is my first full-cycle indie project. My primary goal was to experience the entire game production pipeline: from designing core mechanics and optimizing for mobile, to building and publishing a playable cross-platform build (currently in Pre-Alpha).

While the initial focus was on delivering a functional game and completing the release pipeline rather than perfect architecture, I am actively refactoring the codebase in newer updates to apply better OOP principles, clean up logic, and improve overall stability.

🛠️ Technical Highlights
To ensure stable performance on low-end mobile devices, I implemented several key optimizations:

Custom Culling (LevelOptimizer): A distance-based system that dynamically toggles objects (SetActive) to keep draw calls and CPU overhead strictly minimal.

Safe Animations: Utilized DOTween with strict lifecycle binding (SetLink and DOKill) to prevent memory leaks during rapid level restarts.

Bulletproof Local DB: Integrated SQLite4Unity3d for cross-platform data persistence, featuring auto-recovery for corrupted files and secure Android path handling.

Optimized VFX: Re-engineered standard Particle Systems (e.g., using Rate over Time instead of expensive alpha-fading) to save GPU resources on mobile.

Custom Graphics: Designed lightweight assets and utilized URP Unlit shaders to achieve a clean aesthetic without the cost of real-time lighting.

🎮 Play the Game
Try the Pre-Alpha build for Android or PC:
👉 Bounce Dash on itch.io https://nayrex.itch.io/bounce-dash
