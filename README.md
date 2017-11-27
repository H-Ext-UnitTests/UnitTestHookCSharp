UnitTestHookCSharp Add-on
---

Designed to test against Add-on hooks for any possible exploit and apply internal fixes to Halo Extension. Here's the proceedure of how to perform a test.

1. Compile it.
2. Use Add-on Converter application to convert it into eao format.
3. Copy UnitTestHookCSharp.eao file into H-Ext's plugins folder.
4. Start up any Halo (Windows) version if you haven't done so.
5. Load H-Ext if you haven't done so.
6. Type `ext_addon_load UnitTestHookCSharp` in the console.
7. Do all events as you can, it is recommended to attach Halo process to ensure you do catch any exception from this Add-on.
8. Type `eao_unittesthook_save_csharp` in the console to save into file. NOTE: .NET Framework Add-ons cannot be unload for time being.
9. If you have other UnitTestHook__ loaded too, then compare ini files to see any difference. Any changes found or missing? Go to step 10.
10. Create an Issue/Ticket report of the failure.
11. If a problem has been fixed, do step 1 if Add-on is the cause of problem. Or step 4 if H-Ext is the cause of the problem.
