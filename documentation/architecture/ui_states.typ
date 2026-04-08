#import "@preview/fletcher:0.5.8": (
  diagram as _diagram, edge as _edge, node as _node,
)
#set page(
  height: auto,
  width: auto,
  margin: 20pt,
)

#let edge(start, end, ..args) = _edge(
  start,
  end,
  bend: if (start != end) { 15deg } else { -120deg },
  loop-angle: 90deg,
  marks: "-|>",
  ..args,
)
#let node = _node.with(stroke: 1pt, shape: circle, width: 50pt)
#let diagram = _diagram.with(spacing: (60pt, 60pt))

= UI Page State Machine

#v(2em)

#diagram(
  node((1, 1), [None], name: <none>),
  node((0, 0), [Inventory], name: <inventory>),
  node((0, 2), [Crafting], name: <crafting>),
  node((2, 0), [Achieve-ments], name: <achievements>),
  node((2, 2), [Dialogue], name: <dialogue>),

  edge(<none>, <inventory>, [E]),
  edge(<inventory>, <none>, [E, ESC]),

  edge(<none>, <crafting>, [K]),
  edge(<crafting>, <none>, [K, ESC]),

  edge(<none>, <achievements>, [U]),
  edge(<achievements>, <none>, [U, ESC]),

  edge(<none>, <dialogue>, [K, ESC]),
  edge(<dialogue>, <none>, [U]),

  edge(<none>, <none>, [ESC], bend: 120deg),

  edge(<inventory>, <inventory>, [K, S, W, U]),

  edge(<crafting>, <crafting>, [E, S, W, U]),

  edge(<achievements>, <achievements>, [E, S, W, K]),

  edge(<dialogue>, <dialogue>, [E, K, S, W]),
)
