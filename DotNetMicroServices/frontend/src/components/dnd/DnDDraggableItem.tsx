"use client";

import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import React from "react";

export interface DnDDraggableItemProps {
  children: React.ReactNode;
  draggableId: string;
  getItemStyle?: (isDragging: boolean) => React.CSSProperties;
  disabled?: boolean;
}

const DnDDraggableItem = ({
  children,
  draggableId,
  getItemStyle,
  disabled = false,
}: DnDDraggableItemProps) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: draggableId, disabled });

  const style: React.CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    ...getItemStyle?.(isDragging),
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      {children}
    </div>
  );
};

export { DnDDraggableItem };
export default DnDDraggableItem;
