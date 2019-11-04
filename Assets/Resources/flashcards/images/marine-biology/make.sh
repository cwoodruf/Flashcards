#!/bin/sh
# requires imagemagick but should work with any firefox captured web page image from 
# flashcardmachine such as: https://www.flashcardmachine.com/print/?topic_id=3717699
# saved here as biology-flash-cards-01.jpg
slides=$1
if [ ! -e "$slides" ]
then
    echo needs slides file
    exit 1
fi
shift
name=$(echo $slides | perl -pe 's/\.\w+$//')
ext=$(echo $slides | perl -pe 's/.*\.//')
echo file $slides name $name ext $ext

slidecount=$1
if [ "x$slidecount" = "x" ] 
then
    echo need number of slides
    exit 1
else
    echo "$slidecount" slides
    shift
fi

if [ "x$1" = "x" ]
then
    echo using split as individual image directory
    mkdir -p split
    cd split
else
    echo using "$1" as the individual image directory
    mkdir -p "$1"
    cd "$1"
fi

newsize=$(($slidecount*262))
echo newsize $newsize
convert ../$slides -crop 605x+463+234 -resize 605x$newsize $name-cropped.$ext
identify $name-cropped.$ext
convert $name-cropped.$ext -crop x262 $name-%03d.$ext 
rm $name-cropped.$ext

