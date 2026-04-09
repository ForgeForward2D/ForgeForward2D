#import "@preview/fletcher:0.5.8": (
  diagram as _diagram, edge as _edge, node as _node,
)

#set page(
  height: auto,
  width: auto,
  margin: 20pt,
)

#show raw: it => highlight(fill: gray.lighten(70%), it)

#let marker(fill) = box(
  rect(
    fill: fill,
    height: 1em,
    width: 1em,
  ),
  radius: .3em,
  clip: true,
  baseline: .2em,
)

#let shift = 20pt
#let bend = 15deg

#let desc = it => [\ #text(size: 8pt, fill: orange, it)]

#let edge = _edge.with(marks: "-|>", label-sep: 0pt)
#let input(..args, label: none) = edge(..args, stroke: red, label: if label
  != none { text(fill: red, label) })
// #let attack(..args, label: none) = edge(..args, stroke: orange, label: if label
//   != none { text(fill: orange, label) })
// #let inv(..args, label: none) = edge(..args, stroke: purple, label: if label
//   != none { text(fill: purple, label) })
#let method-call(..args, label: none) = edge(
  ..args,
  stroke: fuchsia,
  label: if label != none { text(fill: fuchsia, label) },
)
#let event(..args, label: none) = edge(..args, stroke: green, label: if label
  != none { text(fill: green, label) })
#let node = _node.with(height: 50pt, stroke: blue)

#let diagram = _diagram.with(
  node-stroke: 1pt,
  spacing: (150pt, 50pt),
)

#let diagram-box(..args, title: none) = box(stroke: 1pt, inset: 1em)[
  #if title != none {
    heading(title)
  }
  #diagram(..args)
]


#diagram-box(
  title: [Movement],
  input((-1, 0), <inputmanager>, label: [WASD]),
  node((0, 0), [Input Manager], name: <inputmanager>),
  event(
    marks: "-",
    label: [On Move Input\ (UI, Vector2Int)],
    label-anchor: "center",
  ),

  node((0.5, 0), stroke: none, name: <onmoveinput>),

  node((1, 0), [Movement Manager#desc[Set velocity]], name: <movementmanager>),
  event(<onmoveinput>, <movementmanager>),
  node(
    (1, 1),
    [Achievement Manager#desc[Update selected index]],
    name: <achievementmanager>,
  ),
  event(<onmoveinput>, <achievementmanager>, corner: left),
  node(
    (1, 2),
    [Crafting Manager#desc[Update selected index]],
    name: <craftingmanager>,
  ),
  event(<onmoveinput>, <craftingmanager>, corner: left),
  node(
    (1, 3),
    [NPC Controller#desc[Update Dialogue Text]],
    name: <npccontroller>,
  ),
  event(<onmoveinput>, <npccontroller>, corner: left),

  node((2, 0), [Achievement Popup Manager], name: <achievementpopupmanager>),
  node((2, 1), [Achievement UI#desc[Refresh UI]], name: <achievementui>),
  event(
    <achievementmanager>,
    <achievementui>,
    label: [OnAchievementMangerUpdate],
    bend: bend,
  ),
  event(
    <achievementui>,
    <achievementmanager>,
    label: [Request\ Refresh],
    bend: bend,
  ),

  node((2, 2), [CraftingTable UI#desc[Refresh UI]], name: <craftingui>),
  event(
    <craftingmanager>,
    <craftingui>,
    label: [OnCraftingManagerUpdate],
    bend: bend,
  ),
  event(
    <craftingui>,
    <craftingmanager>,
    label: [Request\ Refresh],
    bend: bend,
  ),
  node(
    (2, 3),
    [Dialogue UI#desc[Refresh UI]],
    name: <dialogueui>,
  ),

  node((1.5, 4), [Tracker#desc[Record Event]], name: <tracker>),
  event(<npccontroller>, <dialogueui>, label: [OnNPCControllerUpdate]),
  event(<npccontroller>, (1.5, 3), <tracker>),

  event(
    <tracker>,
    (2.5, 4),
    (2.5, -0.5),
    (1.5, -0.5),
    (1.5, 0.3),
    (1, 0.3),
    <achievementmanager>,
    label: [OnTrackerUpdate],
  ),

  event(
    <achievementmanager>,
    (1, 0.5),
    (2, 0.5),
    <achievementpopupmanager>,
    label: [OnAchievement-\ Unlocked],
    label-anchor: "center",
    bend: left,
  ),
)

#diagram-box(
  title: [Attack],
  input((-2, 0), <inputmanager>, label: [_left click_ / J]),
  node((-1, 0), [Input Manager], name: <inputmanager>),

  node((0, 0), [Player Interaction Manager], name: <playerinteractionmanager>),

  node((0, 2), [CraftingTable UI#desc[Refresh UI]], name: <craftingui>),

  node(
    (1, 0),
    [BlockBreaking Manager#desc[Manage Block Breaking Process]],
    name: <blockbreakingmanager>,
  ),
  node((1, 1), [TileMap Manager], name: <tilemapmanager>),
  node((1, 2), [Crafting Manager], name: <craftingmanager>),
  node((1, 3), [Inventory Manager], name: <inventorymanager>),

  node((2, -1), [Tracker#desc[Record Event]], name: <tracker>),
  node(
    (2, 0),
    [Achievement Manager#desc[Track Broken Blocks]],
    name: <achievementmanager>,
  ),
  node(
    (2, 1),
    [Resource Generator#desc[Regenerate Resource]],
    name: <resourcegenerator>,
  ),
  node((2, 2), [Resource Inventory], name: <resourceinventory>),
  node((2, 3), [Hotbar], name: <hotbar>),

  node((3, 0), [Achievement Popup Manager], name: <achievementpopupmanager>),
  node((3, 1), [Achievement UI#desc[Refresh UI]], name: <achievementui>),
  node(
    (3, 2),
    [Resource Inventory UI#desc[Refresh UI]],
    name: <resourceinventoryui>,
  ),
  node((3, 3), [Hotbar UI#desc[Refresh UI]], name: <hotbarui>),
  node(
    (3, 4),
    [PlayerHand Controller#desc[Refresh UI]],
    name: <playerhandcontroller>,
  ),

  node((0.5, 0), stroke: none, name: <onattackinput>),

  event(
    <inputmanager>,
    <playerinteractionmanager>,
    label: [OnAttack-\ InputUpdate],
  ),

  event(
    <playerinteractionmanager>,
    <onattackinput>,
    marks: "-",
    label: [OnAttack\ Update],
    label-anchor: "center",
  ),
  event(
    <onattackinput>,
    <blockbreakingmanager>,
    label-anchor: "north",
  ),
  event(<onattackinput>, <craftingmanager>, corner: left),
  event(<onattackinput>, (0.5, -1), <tracker>),

  event(<tracker>, <achievementmanager>, label: [OnTrackerUpdate]),

  event(<tilemapmanager>, <blockbreakingmanager>, label: [OnBlock\ Changed]),

  event(
    <craftingui>,
    <craftingmanager>,
    label: [Request\ Refresh],
    bend: -bend,
  ),
  event(<craftingmanager>, <craftingui>, label: [OnUpdate], bend: -bend),

  event(
    <inventorymanager>,
    <craftingmanager>,
    label: [OnInventory-\ Update],
    bend: bend,
  ),
  method-call(
    <craftingmanager>,
    <inventorymanager>,
    label: [Add Item],
    bend: bend,
  ),

  node((1.6, 0), stroke: none, name: <onblockbroke>),
  event(
    <blockbreakingmanager>,
    <onblockbroke>,
    label: [OnBlock-\ Broke],
    label-anchor: "center",
    marks: "-",
  ),
  event(<onblockbroke>, <achievementmanager>),
  event(<onblockbroke>, <resourcegenerator>, corner: left),
  event(<onblockbroke>, <resourceinventory>, corner: left),

  node((1.5, 1), stroke: none, name: <drawblock>),
  method-call(
    <blockbreakingmanager>,
    <drawblock>,
    marks: "-",
    corner: right,
    shift: shift,
  ),
  method-call(<resourcegenerator>, <drawblock>, marks: "-"),
  method-call(<drawblock>, <tilemapmanager>, label: [Draw Block]),

  method-call(
    <inventorymanager.east>,
    (1.5, 3),
    (1.5, 2),
    <resourceinventory>,
    label: [Add Item],
    shift: 5pt,
  ),
  method-call(
    <inventorymanager>,
    <hotbar>,
    label: [Add Item],
    label-anchor: "north",
    bend: -bend,
  ),

  event(
    <hotbar>,
    (1.7, 3),
    (1.7, -1),
    (1, -1),
    <blockbreakingmanager>,
    label: [OnUpdate],
    label-pos: 65%,
  ),

  event(
    <achievementmanager>,
    <achievementpopupmanager>,
    label: [OnAchievementUnlocked],
  ),
  event(
    <achievementmanager>,
    (2, 0.5),
    (2.5, 0.5),
    (2.5, 1),
    <achievementui>,
    label: [OnAchievement-\ ManagerUpdate],
  ),

  event(
    <resourceinventory>,
    <resourceinventoryui>,
    label: [OnUpdate],
    bend: -bend,
  ),
  event(
    <resourceinventoryui>,
    <resourceinventory>,
    label: [Request\ Refresh],
    bend: -bend,
  ),

  event(<hotbarui>, <hotbar>, label: [Request\ Refresh], bend: -bend),

  node((2.5, 3), stroke: none, name: <onupdate>),
  event(<hotbar>, <onupdate>, label: [OnUpdate], marks: "-"),
  event(<onupdate>, <hotbarui>),
  event(<onupdate>, <playerhandcontroller>, corner: left),
)

#diagram-box(
  title: [Interact],
  node((0, 0), [Input Manager], name: <inputmanager>),

  node(
    (1, -1),
    [NPC Controller#desc[Update Dialogue Texts]],
    name: <npccontroller>,
  ),
  node(
    (1, 0),
    [PlayerInteraction Manager#desc[Find Block For Interaction]],
    name: <playerinteractionmanager>,
  ),
  node((1, 1), [TileMap Manager], name: <tilemapmanager>),

  node((2, -1), [Dialogue UI#desc[Refresh UI]], name: <dialogueui>),
  node((2, 0), [UI Manager], name: <uimanager>),
  node((2, 2), [Tracker#desc[Record Event]], name: <tracker>),

  node(
    (3, 0),
    [BlockBreaking Manager#desc[Cancel Breaking]],
    name: <blockbreakingmanager>,
  ),
  node(
    (3, 1),
    [Movement Manager#desc[Cancel Walking + Animation]],
    name: <movementmanager>,
  ),
  node(
    (3, 2),
    [Achievement Manager#desc[Check for Achievement]],
    name: <achievementmanager>,
  ),

  input((-1, 0), <inputmanager>, label: [_right click_ / K], shift: 10pt),
  input(
    (-1, 0),
    <inputmanager>,
    label: [E / V / ESC],
    shift: -10pt,
    label-anchor: "north",
  ),

  event(<npccontroller>, <dialogueui>, label: [OnNPCControllerUpdate]),

  node((0.5, 0), stroke: none, name: <oninteractioninput>),
  event(
    <inputmanager>,
    <oninteractioninput>,
    label: [OnInteraction-\ Input],
    marks: "-",
  ),
  event(<oninteractioninput>, <playerinteractionmanager>),
  event(<oninteractioninput>, <npccontroller>, corner: right),

  method-call(
    <playerinteractionmanager>,
    <tilemapmanager>,
    label: [Read Block],
  ),

  node((1.5, 0), stroke: none, name: <oninteraction>),
  event(
    <playerinteractionmanager>,
    <oninteraction>,
    marks: "-",
    label: [OnInteraction],
  ),
  event(<oninteraction>, <uimanager>),
  event(<oninteraction>, <tracker>, corner: left),

  event(
    <inputmanager>,
    (0, 1.7),
    (2, 1.7),
    <uimanager>,
    label: [OnUIChangeInput],
  ),

  node((2.5, 0), stroke: none, name: <onupdatepage>),
  event(
    <uimanager>,
    <onupdatepage>,
    label: [OnUpdate-\ Page],
    label-anchor: "center",
    marks: "-",
  ),
  event(<onupdatepage>, <blockbreakingmanager>),
  event(<onupdatepage>, <movementmanager>, corner: left),
  event(<onupdatepage>, (2.5, -2), (0, -2), <inputmanager>),
  event(<onupdatepage>, <tracker>, corner: right),

  event(
    <tracker.south-west>,
    <achievementmanager.south-west>,
    label: [OnTrackerUpdate],
  ),
)

#diagram-box(
  title: [Hotbar],
  node((0, 0), [Input Manger], name: <inputmanager>),

  node((1, 0), [Hotbar#desc[Update Index]], name: <hotbar>),

  node((2, 0), [Hotbar UI#desc[Update UI]], name: <hotbarui>),
  node(
    (2, 1),
    [PlayerHand Controller#desc[Change Hand hold Object]],
    name: <playerhandcontroller>,
  ),

  input(
    (-1, 0),
    <inputmanager>,
    label: [1 / 2 / 3 / 4 / 5\ _scroll up/down_],
    label-anchor: "center",
  ),

  event(<inputmanager>, <hotbar>, label: [OnHotBarSelect], shift: 10pt),
  event(<inputmanager>, <hotbar>, label: [OnHotBarScroll], shift: -10pt),

  node((1.5, 0), stroke: none, name: <onupdate>),
  event(<hotbar>, <onupdate>, label: [OnUpdate], marks: "-"),
  event(<onupdate>, <hotbarui>),
  event(<onupdate>, <playerhandcontroller>, corner: left),
)

// Legend
#align(right, diagram(
  node(stroke: none, height: 150pt, [
    #align(left)[= Legend:]
    #grid(
      columns: 2,
      column-gutter: 1em,
      row-gutter: 1em,
      marker(red), [Input],
      //   marker(orange), [Attack/Break],
      //   marker(purple), [Inventory],
      marker(fuchsia), [Method call],
      marker(green), [Events],
      marker(orange), [Description],
    )

  ]),
))
