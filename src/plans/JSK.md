# JSK

Thank you so much for the words and the reasoning. I will be forward and attach the files, but the rest of your suggestions ring true.

Next I'm curious how their famously powerful engineering culture works when it shifts to the details: is the bullet point universal? (As a note: he is a remarkable woodworker but would acknowledge that he is still mastering 3-D printing). You may wish to attribute any wisdom to the experienced 3D-printing makers in my community. I'll write this in American Engineer, the goal is to send it in Japanese Engineer (in English).

---

I used an A1, P1P and Anycubic Kobra x to print them. I use Elegoo PLA and PETG which we’ve found to be affordable and fully dependable. Our 3D-printers are well tuned but not skew-calibrated In all, I would say that, for folks buying your plans from the US, our setup is equal or perhaps slightly better than the average-case.
I prepared a .3mf file that

* It was not initially clear to me that some parts needed multiple copies — only once I reached the step to install it. The .3mf file addresses this, but you may consider  adding to the inventory at the top of the document:
  * Print quantity 2 of
* Several parts would benefit from printing in PETG — wherever impact resistance or strength (handles, beams) or creep/heat/vibration (router housing, vacuum attachment). Consider calling that out as an option. Specifically, I was advised to use PETG for: Insert-09-Carriage and Insert-10-Arm; Mortiser-P01-Bearing Block, Mortiser P02/P03-Router Housing, Mortiser P04-Main Block, Mortiser-P29-Fence, and Mortiser P06-Vacuum Adapter.
* I had difficulty getting most heat-set inserts started. It was suggested to have a small 0.3-0.5mm chamfer at each entry: this helped with starting the inserts, establishing the right level, and accomodating material that would have raised a burr.  All vendors of inserts identify the OD, but few specify the tip OD, so allowing for
* I split the [part number] baseplate into two halves with a dovetail to fit on a 250x250 bed

* Since this is targeted for a community maker space I had preemptive concerns that
* Getting metric hardware in the US is not as hard as it once was, but only by buying a full pack of 25+ or an assortment. The quantities for the socket hardware were easily justified, and satisfied, by buying assortment packs. However, the knobs required buying small quantities of different hex-head bolts at diffferent lengths
  * I suggest offering a separate download with knobs and sleeve adapters to allow using a .8-32 (
  * manufacturers dependably use the same outer dimensions as M4 / M5 inserts.

* H01-Main Base
* H02-Tower
  * Front: eight M3 inserts with eight M3 x 10mm socket bolts; for slide;
  * Back: two M3 inserts with one M3 x 10mm socket bolts for spring and  one M3 x 5 screw(§) for cover;
  * Bottom: two M4 6mm OD x 5mm L inserts with two M4 x 20mm screws, for base;
  * Optional: two 6mm x 3mm magnets, north inward, for caddy
* H03-Pulley Cap
  * two pulleys
  * two M5 x 5mm(!) inserts with two M5 x 25mm socket bolts for pulleys.
  * one M3 insert with one M3 x 5mm screw(§) for cover
* H04-Tower Back Cover
* H05-Insert Adapter Caddy
  * six M4 6mm OD x 5mm L inserts
  * Optional: two 6mm x 3mm magnets, north outward
* H06-Belt-Spring Tab accepts belt and
  * one M2 x 2.5mm(!) insert with one M2 x 5mm countersunk screw, for belt
* H07-Smooth M4 Knob (print **four copies**)
* H08-Smooth M4 Knob Cover (print **four copies**)
* H09-Arm Carriage (*)
* H10-Arm Beam (*), use with
  * H09-Arm Carriage
  * four M3 x 8mm Socket Head Screws
  * one H07/H08-M4 Knob with a M4 x 20mm External Hex Bolt
  * one H07/H08-M4 Knob with an M4 Nut
  * soldering iron
  * (optional) H14-Mini-Drill Sleeve and Mini-drill
* H11-Drawer Handle (optional, uses two M3 inserts and two M3 x 10mm low-profile socket bolts (§))
* H12-Drawer Base (optional)
* H13-Drawer Tray (optional)
* H14-Mini-Drill Sleeve (optional)
* H15-Z-lock Left (see next)
* H16-Z-lock Right use with
  * H15-Z-lock Left
  * one H07/H08-M4 Knob with a M4 x 30mm External Hex Bolt
  * one H07/H08-M4 Knob with an M4 Nut

(*) Consider printing these parts in PETG
(§) May substitute these fasteners, see below

Acceptable fastener substitutions
* With H06 Belt Tab, you may instead use US .2 countersunk screw with US .2 insert (0.100inch length, 0.100 inch OD)
* With H13/H11-Drawer Tray/Handle: a low-profile socket bolt will give the best appearance, but you may substitute a regular screw or a US#4-40 screw/insert
* With H10/H08/H07 and H15/H16/H07/H08 Knobs: to instead use US#6-32 bolts, skip the four H07; print four H07-Knob-USno6 and two H09-Sleeve-USno6. For 30mm length, use 1+1/4” bolts; for 20mm length, use 7/8” or 3/4” bolts
Note: If you change a screw you must also change the matching insert or nut if any

* 4.51 / 2.41 (M3 2.65 US#6-32 2.6416)
* 4.00 (M5? 4.26) / 1.95 (M2.5 2.1ish, US#4-40 2.159)
* 3.22 (M4 3.46 US.8-32 3.302) / 1.44 (M2 1.72ish, US#2 1.7018)
* 3.71 (US#10 3.683)
* 4.9 (US1/4-20 4.9784)
* 4.98 (M6 5.03)
* 6.6 (US5/16-18 6.4008)
* No ?? US#10-24 3.683 ?? or 10-32 3.9624 ?
* No US3/8-16 7.7978

# Parts List

Please print the following parts, some of which need multiple copies. Pay close attention to the orientation shown in the photos (the attached .3mf file lets you print directly with recommended per-part settings).

### Tool and Base Parts (P01-P13)

1. **P01-Bearing Block (Front)** and **P01-Bearing Block (Back)** — Print **two copies** (one for the front, one for the back), vertically; PETG recommended.
  - Each takes two V05-Linear Bushing 8mm ID (four total), riding the V01-Side-to-Side Shaft 8mm x 116mm supported by that end of P04-Main Body
  - Together, they support the two V02-In-Out Shaft on which P02-Lower Motor Housing slides
  - Captures V06-Spring VUR10-70 on the V02-In-Out Shaft (Left)
  - Captures V07-Set Collar 8mm ID on the V02-In-Out Shaft (Right)

2. **P02-Lower Motor Housing** — Print on the side; PETG recommended. Takes two M6 Thin Square Nut (DIN 562, 2.72mm thick) and four V05-Linear Bushing 8mm ID, sliding on the two V02-In-Out Shaft supported by P01-Bearing Block (Front)/(Back).

3. **P03-Upper Motor Housing** — Print on the side; PETG recommended. Takes two M4 x 6mm OD x 5mm L Heat Insert for P05-Main Handle, and bolts through the P02-Lower Motor Housing square nuts using two M6x50mm Socket Head Screw — this sandwich encloses the motor.

4. **P04-Main Body** — Print on the side. Takes:
  - the two V01-Side-to-Side Shaft 8mm x 116mm, on which P01-Bearing Block (Front)/(Back) ride
  - three M3x15mm Countersunk Screw, into the three Heat Inserts in P06-Vacuum Adapter
  - four M6x10mm Low-Profile Socket Head Screw when attached to P15-Riser Mount or T05-11-Tilter Sled
  - four P13-Shaft Hole Cover, glued on after the V01-Side-to-Side Shaft are inserted

5. **P05-Main Handle** — Attaches to P03-Upper Motor Housing with two M4x10mm Socket Head Screw. Print at an angle using support; PETG recommended.

6. **P06-Vacuum Adapter** — Print vertically with support; PETG recommended. Takes three M3 x 5mm OD x 4mm L Heat Insert.

7. **P07-Straight Fence** — Print with the large flat face down. Takes:
  - two M4 x 6mm OD x 5mm L Heat Insert
  - two P11/P12-Fence/Body Knob Long M4x35mm assemblies, attaching it to P04-Main Body
  - one M4x20mm Countersunk Screw, into the Heat Insert in P08-Handle for Straight Fence
  - two P10-Fence Hole Cover, glued

8. **P08-Handle for Straight Fence** and **P08-Handle for Angle Fence** — Print **two copies** (one for P07-Straight Fence, one for P28/P29-Angle Fence) with the large face down. Each takes one M4 x 6mm OD x 6mm L Heat Insert and receives one M4x20mm Countersunk Screw from its mating fence piece.

9. **P09-Fence Handle Cover** — Print **two copies** in second color with the flat face down. Glues to P08-Handle for Straight Fence and P08-Handle for Angle Fence.

10. **P10-Fence Hole Cover** — Print **four copies** in second color with the flat face down. Two glue into P07-Straight Fence, two into P28-Angle Fence Brow.

11. **P11-Fence/Body Knob Long M4x35mm** — Print **two copies** with the large face down; no support needed. (Only two of these are needed, as they can be used with either fence style.) Each assembly uses:
  - one P11-Fence/Body Knob Long M4x35mm
  - one P12-Knob M4 Cover
  - one M4x35mm External Hex Bolt
  - one M4 Large Washer

12. **P12-Knob M4 Cover** — Print **two copies** in second color with the larger face down. Used with P11-Fence/Body Knob Long M4x35mm.

13. **P13-Shaft Hole Cover** — Print **four copies** in second color with the larger face down. Glues to P04-Main Body after the V01-Side-to-Side Shaft are inserted.

### Fixed Base Elevating Table (P14-P25)

14. **P14-Wood Footing** — Make from wood, MDF or any suitable flat material; plywood 12mm (1/2") is recommended. Place holes by printing the plans onto paper for use as a drilling template for twelve M6 Threaded Insert. Alternatively, a laser cutter can help mark or drill the holes. Receives four M6x20mm Socket Head Screw from P16-Sliding Table Base (you'll use different sets of the inserts based on its position) and four M6x20mm Socket Head Screw from either the P15-Riser Mount or the T01-11-Tilter Mount.

15. **P15-Riser Mount** — Print flat side down.
  - Takes four M6 x 8mm OD x 8mm L Heat Insert, receiving the four M6x10mm Low-Profile Socket Head Screw from P04-Main Body.
  - Mounts directly to P14-Wood Footing using four M6x20mm Socket Head Screw. Can be swapped out for T01-T11 Tilter assembly to hold the tool at an aligned angle.

16. **P16-Sliding Table Base** — Print with the flat face down; alternate files are available to print it in halves. Attaches to P14-Wood Footing with four M6x20mm Socket Head Screw and four M6 Large Washer.

17. ~~P17-Table Stair Bottom (L)~~ — removed; integrated into P16-Sliding Table Base

18. ~~P18-Table Stair Bottom (R)~~ — removed; integrated into P16-Sliding Table Base

19. **P19-Table Stair Top (L)** — see P20-Table Stair Top (R).

20. **P20-Table Stair Top (R)** — Print largest flat face down. P19/P20 use sixteen M4 x 6mm OD x 6mm L Heat Insert: five each on the top to attach P21-Table Top A, P22-Table Top B, and the V08-Aluminum T-Track; three each on the side for a pair of P24/P25-Table Stair Knob M4x20mm assemblies. (There are multiple inserts for each knob for a variety of positions)

21. **P21-Table Top A** — Make from 12mm thick hardwood (you may substitute any material with an actual thickness near 1/2"). Secure to P19/P20 with four M4x20mm Countersunk Screw.

22. **P22-Table Top B** — As P21-Table Top A, but less broad. Secure to P19/P20 with four M4x20mm Countersunk Screw.

23. **P23-Table Track Support** — Make one, from 12mm thick hardwood or MDF (you may substitute any material with an actual thickness near 1/2"). Glues to the underside of P21-Table Top A and P22-Table Top B and supports the V08-Aluminum T-Track, which is secured to P19/P20 with two M4x10mm Countersunk Screw.

24. **P24-Table Stair Knob M4x20mm** and **P24-Fence Beak/Brow Knob M4x20mm** — two parts from one identical model. Print **six copies** with the flat face down (four for P19/P20-Table Stairs, two for P28/P29-Angle Fence). Each assembly uses:
  - one P24-Knob M4x20mm (this part)
  - one P25-Knob M4 Cover
  - one M4x20mm External Hex Bolt

25. **P25-Knob M4 Cover** — Print **six copies** in second color with the flat face down. Used with P24-Table Stair Knob M4x20mm and P24-Fence Beak/Brow Knob M4x20mm.

### Portable Angle Fence and Accessories (P26-P30)

26. **P26-Slide Limiter 9.8mm** — Print **two copies** flat. Slides onto the front V01-Side-to-Side Shaft to make this a dowel jig.

27. **P27-Slide Limiter 3.0mm** — Print **two copies** flat. Slides onto the front V01-Side-to-Side Shaft to adapt for thinner dominos.

28. **P28-Angle Fence Brow** — Print flat.
  - In front, takes two M4 x 6mm OD x 5mm L Heat Insert for two P11/P12-Fence/Body Knob Long M4x35mm assemblies
  - Each side (top recess) takes one M4 x 6mm OD x 6mm L Heat Insert
  - Attaches to P29-Angle Fence Beak with two P24/P25-Fence Beak/Brow Knob M4x20mm assemblies, two V09-4mmx20mm Dowel Pin, and two P30-Dowel Pin Cap

29. **P29-Angle Fence Beak** — Print flat, using support for the two upper arcs (see photo). Uses:
  - two V09-4mmx20mm Dowel Pin (shared with P28-Angle Fence Brow)
  - one M4x20mm Countersunk Screw to attach P08-Handle for Angle Fence
  - two P24/P25-Fence Beak/Brow Knob M4x20mm assemblies (shared with P28-Angle Fence Brow)

30. **P30-Dowel Pin Cap** — Print **two copies** standing up, mouth down. Used with the two V09-4mmx20mm Dowel Pin.

### Tilting Mount Parts (T01-T11)

This whole assembly swaps in for P15-Riser Mount, re-using its mounting screws, to allow placing biscuits in miter joints (ones that meet at non-right angle).

1. **T01-Tilter Mount (L)** — print flat. It swaps in for P15-Riser Mount and reuses the same M6x20mm Socket Head Screw.

2. **T02-Tilter Mount (R)** — print flat. It swaps in for P15-Riser Mount and reuses the same M6x20mm Socket Head Screw.

3. **T03-Tilter Knob M5x25mm** — print **two copies**, larger face down. Mounts into T01/T02-Tilter Mount along with:
  - T04-Knob M5 Cover
  - M5x25mm External Hex Bolt
  - M5 Large Washer

4. **T04-Knob M5 Cover** — print **two copies** flat-face down.

5. **T05-11-Tilter Sled** — print flat. Takes:
  - two M5 Nut, that receive the T03/T04-Tilter Knob M5x25mm assemblies
  - four M6 Nut, that receive the M6x10mm Low-Profile Socket Head Screw from P04-Main Body


# Inventory and Buy List

- [x] Makita RT50DZ 18V Cordless Trimmer — one (alternative motor mounts available for different brands)
- [x] 3D Printer Filament PLA (Optional: PETG for some parts)
- [ ] Motion Control:
  - [x] **V01-Side-to-Side Shaft 8mm x 116mm** — two (for P04-Main Body, carrying P01-Bearing Block (Front)/(Back))
  - [x] **V02-In-Out Shaft 8mm x 130mm** — two (spanning P01-Bearing Block (Front)/(Back), carrying P02-Lower Motor Housing)
  - [x] **V05-Linear Bushing 8mm ID** (15mm OD x 24mm L) — eight
    - [x] two each for P01-Bearing Block (Front)/(Back)
    - [x] four for P02-Lower Motor Housing
  - [ ] ? **V06-Spring VUR10-70** — one (for V02-In-Out Shaft (Left))
  - [ ] ? **V07-Set Collar 8mm ID** — one (for V02-In-Out Shaft (Right)); may be printable
  - [ ] ? **V08-Aluminum T-Track** HFSPUR6-1220-230 — one (for P21/P22/P23-Table Top)
  - [ ] **V09-4mmx20mm Dowel Pin** — two (shared by P28-Angle Fence Brow and P29-Angle Fence Beak) — [4mm x 20mm A1 Stainless Steel ISO 2338 Precision Dowel Pin](https://www.fastenal.com/product/details/11511389), SKU 11511389
- [ ] Heat Inserts:
  - [ ] M3 x 5mm OD x 4mm L Heat Insert — three (for P06-Vacuum Adapter)
  - [ ] M4 x 6mm OD x 5mm L Heat Insert — six
    - [ ] two for P03-Upper Motor Housing
    - [ ] two for P07-Straight Fence
    - [ ] two for P28-Angle Fence Brow
  - [ ] M4 x 6mm OD x 6mm L Heat Insert — twenty
    - [ ] one each for P08-Handle for Straight Fence and P08-Handle for Angle Fence
    - [x] five each for the tops of P19/P20-Table Stair Top
    - [x] three each for the sides of P19/P20-Table Stair Top
    - [ ] two for the side recesses of P28-Angle Fence Brow
  - [x] M6 x 8mm OD x 8mm L Heat Insert — four (for P15-Riser Mount)
- [ ] Threaded Inserts:
  - [x] M6 Threaded Insert — twelve (for P14-Wood Footing)
    - [ ] four for the mount
    - [ ] eight in two parallel rows of four, giving P16-Sliding Table Base its range of positions
- [ ] M3 Fasteners:
  - [ ] M3x15mm Countersunk Screw — three (from P04-Main Body into P06-Vacuum Adapter)
- [ ] M4 Fasteners:
  - [ ] M4x10mm Socket Head Screw — two (for P05-Main Handle)
  - [ ] M4x10mm Countersunk Screw — two (securing V08-Aluminum T-Track to P19/P20-Table Stair Top)
  - [ ] M4x20mm Countersunk Screw — ten
    - [ ] one for P07-Straight Fence into P08-Handle for Straight Fence
    - [ ] one for P29-Angle Fence Beak into P08-Handle for Angle Fence
    - [ ] four for P21-Table Top A
    - [ ] four for P22-Table Top B
  - [ ] M4x20mm External Hex Bolt — six — [M4-0.7 x 20mm DIN 933 Class 8.8 Zinc Finish Hex Cap Screw](https://www.fastenal.com/product/details/38525), SKU 38525
    - [ ] four for P24/P25-Table Stair Knob M4x20mm
    - [ ] two for P24/P25-Fence Beak/Brow Knob M4x20mm
  - [ ] M4x35mm External Hex Bolt — two (for P11/P12-Fence/Body Knob Long M4x35mm) — [M4-0.7 x 35mm Class 8.8 Zinc Finish Hex Cap Screw ISO 4014 (DIN 931)](https://www.fastenal.com/product/details/38528), SKU 38528
  - [ ] M4 Large Washer — two (for P11/P12-Fence/Body Knob Long M4x35mm). A variety of sizes near 12mm (1/2") OD for either M4 or US #8 will be suitable.
- [ ] M5 Fasteners:
  - [ ] M5x25mm External Hex Bolt — two (for T03/T04-Tilter Knob M5x25mm)
  - [ ] M5 Large Washer — two (for T03/T04-Tilter Knob M5x25mm). A variety of sizes near 20mm (5/8" to 3/4") OD for either M5 or US #10 will be suitable.
  - [ ] M5 Nut — two (for T05-11-Tilter Sled, receiving the T03/T04-Tilter Knob M5x25mm assemblies)
- [ ] M6 Fasteners:
  - [ ] M6 Thin Square Nut (DIN 562, 2.72mm thick) — two (for P02-Lower Motor Housing, receiving the M6x50mm Socket Head Screw from P03-Upper Motor Housing)
  - [ ] M6 Nut — four (for T05-11-Tilter Sled, receiving the M6x10mm Low-Profile Socket Head Screw from P04-Main Body)
  - [ ] M6x10mm Low-Profile Socket Head Screw — four (from P04-Main Body into P15-Riser Mount or T05-11-Tilter Sled)
  - [ ] M6x20mm Socket Head Screw — eight
    - [ ] four for P16-Sliding Table Base
    - [ ] four for P15-Riser Mount into P14-Wood Footing — reused for T01/T02-Tilter Mount
    - [ ] Alternative: four M6x20mm External Hex Bolt to use with extra P24/P25 knobs
  - [ ] M6x50mm Socket Head Screw — two (from P03-Upper Motor Housing into the M6 Thin Square Nut in P02-Lower Motor Housing)
  - [ ] M6 Large Washer — four (for P16-Sliding Table Base). A variety of sizes near 20mm (5/8" to 3/4") OD for either M6 or US 1/4" will be suitable.

### Removed Pieces

No longer required following part integration; listed so earlier build notes and BOMs reconcile.

- ~~M4x15mm Countersunk Screw — twelve~~ (eight for T05-T11 Tilter Sled, now integral as T05-11-Tilter Sled)
- ~~M4 Nut — twelve~~ (eight for T05-T11 Tilter Sled, now integral as T05-11-Tilter Sled)
- ~~M6 x 8mm OD x 8mm L Heat Insert — ten~~ (for P17-Table Stair Bottom (L) and P18-Table Stair Bottom (R), now integrated into P16-Sliding Table Base)
- ~~M6x20mm Countersunk Screw — ten~~ (for P17-Table Stair Bottom (L) and P18-Table Stair Bottom (R), now integrated into P16-Sliding Table Base)
