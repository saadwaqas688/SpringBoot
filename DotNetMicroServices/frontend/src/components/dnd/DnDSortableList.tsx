"use client";

import { SortableContext, SortableContextProps } from "@dnd-kit/sortable";

export interface DnDSortableListProps
  extends Omit<SortableContextProps, "children"> {
  children: React.ReactNode;
}

const DnDSortableList = ({
  children,
  ...sortableProps
}: DnDSortableListProps) => {
  return <SortableContext {...sortableProps}>{children}</SortableContext>;
};

export { DnDSortableList };
export default DnDSortableList;

