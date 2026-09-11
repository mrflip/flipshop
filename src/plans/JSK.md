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
  * I suggest offering a separate download with knobs and sleeve adapters to allow using a \#8-32 (
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
* With H06 Belt Tab, you may instead use US \#2 countersunk screw with US \#2 insert (0.100inch length, 0.100 inch OD)
* With H13/H11-Drawer Tray/Handle: a low-profile socket bolt will give the best appearance, but you may substitute a regular screw or a US#4-40 screw/insert
* With H10/H08/H07 and H15/H16/H07/H08 Knobs: to instead use US#6-32 bolts, skip the four H07; print four H07-Knob-USno6 and two H09-Sleeve-USno6. For 30mm length, use 1+1/4” bolts; for 20mm length, use 7/8” or 3/4” bolts
Note: If you change a screw you must also change the matching insert or nut if any

* 4.51 / 2.41 (M3 2.65 US#6-32 2.6416)
* 4.00 (M5? 4.26) / 1.95 (M2.5 2.1ish, US#4-40 2.159)
* 3.22 (M4 3.46 US\#8-32 3.302) / 1.44 (M2 1.72ish, US#2 1.7018)
* 3.71 (US#10 3.683)
* 4.9 (US1/4-20 4.9784)
* 4.98 (M6 5.03)
* 6.6 (US5/16-18 6.4008)
* No ?? US#10-24 3.683 ?? or 10-32 3.9624 ?
* No US3/8-16 7.7978

## Parts List

Please print the following parts, some of which need multiple copies. Pay close attention to the orientation shown in the photos (the attached .3mf file lets you print directly with recommended per-part settings).

### Tool and Base Parts (P01-P13)

1\. **P01-Bearing Block (Front)** and **P01-Bearing Block (Back)** — Print **two copies** (one for the front, one for the back), vertically; PETG recommended.
* Each takes two Linear Bushing 8mm ID (four total), riding the Side-to-Side Shaft 8mm x 116mm supported by that end of P04-Main Body
* Together, they support the two In-Out Shafts on which P02-Lower Motor Housing slides
* Captures Spring VUR10-70 on the In-Out Shaft 8mm x 130mm (Left)
* Captures Set Collar 8mm ID on the In-Out Shaft 8mm x 130mm (Right)

2\. **P02-Lower Motor Housing** — Print on the side; PETG recommended. Takes two M6 Thin Square Nut (DIN 562, 2.72mm thick) and four Linear Bushing 8mm ID, sliding on the two In-Out Shafts supported by P01-Bearing Block (Front)/(Back).

3\. **P03-Upper Motor Housing** — Print on the side; PETG recommended. Takes two M4 x 6mm OD x 5mm L Heat Insert for P05-Main Handle, and bolts through the P02-Lower Motor Housing square nuts using two M6x50mm Socket Head Screws — this sandwich encloses the motor.

4\. **P04-Main Body** — Print on the side. Takes:
* the two Side-to-Side Shaft 8mm x 116mm, on which P01-Bearing Block (Front)/(Back) ride
* three M3x15mm Countersunk Screws, into the three Heat Inserts in P06-Vacuum Adapter
* four M6x10mm Low-Profile Socket Head Screws when attached to P15-Riser Mount or T05-11-Tilter Sled
* four P13-Shaft Hole Cover, glued on after the Side-to-Side Shafts are inserted

5\. **P05-Main Handle** — Attaches to P03-Upper Motor Housing with two M4x10mm Socket Head Screw. Print at an angle using support; PETG recommended.

6\. **P06-Vacuum Adapter** — Print vertically with support; PETG recommended. Takes three M3 x 5mm OD x 4mm L Heat Insert.

7\. **P07-Straight Fence** — Print with the large flat face down. Takes:
* two M4 x 6mm OD x 5mm L Heat Insert
* two P11/P12-Fence/Body Knob Long M4x35mm assemblies, attaching it to P04-Main Body
* one M4x20mm Countersunk Screw, into the Heat Insert in P08-Handle for Straight Fence
* two P10-Fence Hole Cover, glued

8\. **P08-Handle for Straight Fence** and **P08-Handle for Angle Fence** — Print **two copies** (one for P07-Straight Fence, one for P28/P29-Angle Fence) with the large face down. Each takes one M4 x 6mm OD x 6mm L Heat Insert and receives one M4x20mm Countersunk Screw from its mating fence piece.

9\. **P09-Fence Handle Cover** — Print **two copies** in second color with the flat face down. Glues to P08-Handle for Straight Fence and P08-Handle for Angle Fence.

10\. **P10-Fence Hole Cover** — Print **four copies** in second color with the flat face down. Two glue into P07-Straight Fence, two into P28-Angle Fence Brow.

11\. **P11-Fence/Body Knob Long M4x35mm** — Print **two copies** with the large face down; no support needed. (Only two of these are needed, as they can be used with either fence style.) Each assembly uses:
- one P11-Fence/Body Knob Long M4x35mm
* one P12-Knob M4 Cover
* one M4x35mm External Hex Bolt
* one M4 Large Washer

12\. **P12-Knob M4 Cover** — Print **two copies** in second color with the larger face down. Used with P11-Fence/Body Knob Long M4x35mm.

13\. **P13-Shaft Hole Cover** — Print **four copies** in second color with the larger face down. Glues to P04-Main Body after the Side-to-Side Shafts are inserted.

### Fixed Base Elevating Table (P14-P25):

14\. **P14-Wood Footing** — Make from wood, MDF or any suitable flat material; plywood 12mm (1/2") is recommended. Place holes by printing the plans onto paper for use as a drilling template for twelve M6 Threaded Inserts. Alternatively, a laser cutter can help mark or drill the holes. Receives four M6x20mm Socket Head Screws from P16-Sliding Table Base (you’ll use different sets of the inserts based on its position) and four M6x20mm Socket Head Screws from either the P15-Riser Mount or the T01-11-Tilter Mount.


15\. **P15-Riser Mount** — Print flat side down.
- Takes four M6 x 8mm OD x 8mm L Heat Insert, receiving the four M6x10mm Low-Profile Socket Head Screws from P04-Main Body.
- Mounts directly to P14-Wood Footing using four M6x20mm hex socket head bolts. Can be swapped out for T01-T11 Tilter assembly to hold the tool at an aligned angle.

16\. **P16-Sliding Table Base** — Print with the flat face down; alternate files are available to print it in halves. Attaches to P14-Wood Footing with four M6x20mm Socket Head Screws and four M6 Large Washers.

17\. ~~~~P17-Table Stair Bottom (L)~~~~ — removed

18\. ~~~~P18-Table Stair Bottom (R)~~~~ — removed

19\. **P19-Table Stair Top (L)** — see P20-Table Stair Top (R).

20\. **P20-Table Stair Top (R)** — Print largest flat face down. P19/P20 use sixteen M4 x 6mm OD x 6mm L Heat Insert: five each on the top to attach P21-Table Top A, P22-Table Top B, and the Aluminum T-Track; three each on the side for a pair of P24/P25-Table Stair Knob M4x20mm assemblies. (There are multiple inserts for each knob for a variety of positions)

21\. **P21-Table Top A** — Make from 12mm thick hardwood (you may substitute any material with an actual thickness near 1/2"). Secure to P19/P20 with four M4x20mm Countersunk Screws.

22\. **P22-Table Top B** — As P21-Table Top A, but less broad. Secure to P19/P20 with four M4x20mm Countersunk Screw.

23\. **P23-Table Track Support** — Make one, from 12mm thick hardwood or MDF (you may substitute any material with an actual thickness near 1/2"). Glues to the underside of P21-Table Top A and P22-Table Top B and supports the T-Track.

24\. **P24-Table Stair Knob M4x20mm** and **P24-Fence Beak/Brow Knob M4x20mm** — two parts from one identical model. Print **six copies** with the flat face down (four for P19/P20-Table Stairs, two for P28/P29-Angle Fence). Each assembly uses:
    * one P24-Knob M4x20mm (this part)
    * one P25-Knob M4 Cover
    * one M4x20mm External Hex Bolt

25\. **P25-Knob M4 Cover** — Print **six copies** in second color with the flat face down. Used with P24-Table Stair Knob M4x20mm and P24-Fence Beak/Brow Knob M4x20mm.

### Portable Angle Fence and Accessories (P26-P30):

26\. **P26-Slide Limiter 9.8mm** — Print **two copies** flat. Slides onto the front Side-to-Side Shaft to make this a dowel jig.

27\. **P27-Slide Limiter 3.0mm** — Print **two copies** flat. Slides onto the front Side-to-Side Shaft to adapt for thinner dominos.

28\. **P28-Angle Fence Brow** — Print flat.
    * In front, takes two M4 x 6mm OD x 5mm L Heat Insert for two P11/P12-Fence/Body Knob Long M4x35mm assemblies
    * Each side (top recess) takes one M4 x 6mm OD x 6mm L Heat Insert
    * Attaches to P29-Angle Fence Beak with two P24/P25-Fence Beak/Brow Knob M4x20mm assemblies, two Dowel Pin 4mm OD x 20mm L, and two P30-Dowel Pin Cap

29\. **P29-Angle Fence Beak** — Print flat, using support for the two upper arcs (see photo). Uses:
    * two Dowel Pin 4mm OD x 20mm L (shared with P28-Angle Fence Brow)
    * one M4x20mm Countersunk Screw to attach P08-Handle for Angle Fence
    * two P24/P25-Fence Beak/Brow Knob M4x20mm assemblies (shared with P28-Angle Fence Brow)

30\. **P30-Dowel Pin Cap** — Print **two copies** standing up, mouth down. Used with the two Dowel Pin 4mm OD x 20mm L.

### Tilting Mount Parts (T01-T11):
This whole assembly swaps in for P15-Riser Mount, re-using its mounting screws, to allow placing biscuits in miter joints (ones that meet at non-right angle).


1\. **T01-Tilter Mount (L)** — print flat. It swaps in for P15-Riser Mount and rreuses the same M6x20mm Socket Head Screws
2\. **T02-Tilter Mount (R)** — print flat. It swaps in for P15-Riser Mount and reuses the same M6x20mm Socket Head screws
3\. **T03-Tilter Knob M5x20mm** — print **two copies**, larger face down. Mounts into T01/T02-Tilter Mount along with:
* T04-Knob M5 Cover
* M5x20mm External Hex Bolt
* M5 Large Washer
4\. **T04-Knob M5 Cover**  — print  **two copies** flat-face down.
5\. **T05-11-Tilter Sled** — print flat. Takes
* two regular M5 Nuts, that receive the T03-Tilter Knob M5x20mm assembly.
* four regular M6 Nuts, that receive the M6x10mm Low-Profile Socket Head Screws from the main body.

## Inventory and Buy List:

- [x] Makita RT50DZ 18V Cordless Trimmer x 1 (alternative motor mounts available for different brands)
- [x] 3D printer filament PLA
- [ ] Motion control:
  - [x] Φ8mm, 116mm length shaft x 2
  - [x] Φ8mm, 130mm long shaft x 2
  - [x] Linear bushings: inner diameter 8mm, outer diameter 15mm, length 24mm x 8 (two each for P01-Bearing Block A / P01-Bearing Block B; four for P03-Upper Motor Housing)
  - [ ] ordered?  Spring (VUR10-70) x 1
  - [ ] printable? 8mm inner diameter set collar x 1
  - [ ] Aluminum T-Track HFSPUR6-1220-230 ×1 (for P21/22/23 Table Top)
  - [ ] Φ4×20mm metal Dowel Pins　× 2 (two for P29-Angle Fence Beak) [M4 x 20mm A1 Stainless Steel ISO 2338 Precision Dowel Pin](~https://www.fastenal.com/product/details/11511389~) **SKU :** 11511389
- [ ] Heat-Press Threaded Inserts:
  - [ ] M3 x 5mm OD 4mm L insert nuts x 3 (three for P06-Vacuum Adapter)
  - [ ] M4 x 6mm OD 5mm L insert nuts x 4 (two for P03-Upper Motor Housing, two for P28-Angle Fence Brow, two for P07-Straight Fence)
  - [ ] M4 x 6mm OD 6mm L insert nuts x 18 (one for P08-Straight Fence Handle, one for P08-Angle Fence Handle, five each for upper part of P19/P20-Table Stair Top, three each for sides of P19/P20-Table Stair Top)
  - [ ] M6 x 8mm OD 8mm L insert nuts x 14 (four for P15-Riser Mount)
  - [ ] M6 threaded insert nuts x 12 (twelve for P14-Wood Footing)
- [ ] M3 Fasteners:
  - [ ] M3 x 15mm countersunk nuts x 3 (three for P06-Vacuum Adapter-Main Body)
- [ ] M4 Fasteners:
  - [ ] M4 x 10mm socket-head screws x 2 (two for P05-Main Handle)
  - [ ] M4 x 15mm countersunk screws x 12 (~~~~eight for T05-11 Tilter Sled~~~~)
  - [ ] M4 x 10mm countersunk screws x 2
    - [ ] two for securing the T-Track to the stairs top
  - [ ] M4 x 20mm countersunk screws x 10
    - [ ] one for P29-Angle Fence Beak/P08-Angle Fence Handle
    - [ ] one for P07-Straight Fence /P08-Straight Fence Handle
    - [ ] eight for securing P21-Table Top A/B & the T-Track to the stairs top
  - [ ] M4 x 20mm external hex bolts x 6 — [M4-0.7 x 20mm DIN 933 Class 8.8 Zinc Finish Hex Cap Screw](~https://www.fastenal.com/product/details/38525~) SKU: 38525
    - [ ] two for P24-Knob M4 20mm (for P28/P29-Angle Fence)
    - [ ] four for P24-Knob M4 20mm (for the stairs)
  - [ ] M4 x 35mm external hex bolts x 2 — [M4-0.7 x 35mm Class 8.8 Zinc Finish Hex Cap Screw ISO 4014 \(DIN 931\)](~https://www.fastenal.com/product/details/38528~) SKU 38528
    - [ ] two for P11-Fence Sliding M4 35mm Knob
  - [ ] M4 Large Washer ×2 (two for P11-Fence Sliding M4 35mm Knob). A variety of sizes near 12mm (1/2inch) OD for either M4 or US#8 will be suitable.
  - [ ] M4 nuts × 12 (~~~~eight for T05-11 Tilter Sled~~~~)
- [ ] M5 Fasteners:
  - [ ] M5 nuts ×2 (in T11 - Tilter Sled)
  - [ ] M5 x 25mm external hex bolt ×2 (two T03 - Knob M5 25mm)
  - [ ] M5 Large Washer x 2 (two T03 - Knob M5 25mm). A variety of sizes near 20mm (5/8 to 3/4inch) OD for either M5 or US#10 will be suitable.
- [ ] M6 Fasteners:
  - [x] M6 square nuts x 2 (two for P02-Lower Motor Housing)
  - [x] M6 nuts × 4 (~~~~four for T05-11 Tilter Sled~~~~)
  - [ ] M6 x 50mm screws x 2
  - [ ] M6 x 20mm countersunk screws x 10
  - [ ] M6 x 20mm hex socket head bolts x 8 (four for P16-Sliding Table Base, four for P15-Riser Mount — reused for T01/T02 Tilter Mount)
    - [ ] (Alternative: four M6 x 20 mm external hex bolts to use with extra P24/P25-knobs)
  - [ ] M6 x 10mm low-profile socket-head screws x 4
  - [ ] M6 Large Washer x 4 (four for P16-Sliding Table Base). A variety of sizes near 20mm (5/8 to 3/4inch) OD for either M6 or US#1/4inch will be suitable.
