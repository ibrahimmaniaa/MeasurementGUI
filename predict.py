import cv2
import torch
import os
import numpy as np
#from time import time
from PIL import Image
from networks.dinknet import DinkNet34
from framework import MyFrame
from loss import dice_bce_loss
import sys


def smoothing(img, iter, filter_size):
    dst = None

    for j in range(iter):
        kernel = np.ones((filter_size, filter_size), np.float32) / (filter_size * filter_size)
        dst = cv2.filter2D(img, -1, kernel)
        img = dst

    return dst


def sharpening(img, iter):
    dst = None

    for j in range(iter):
        kernel = np.array([[0, -1, 0],
                           [-1, 5, -1],
                           [0, -1, 0]])
        dst = cv2.filter2D(img, -1, kernel)
        img = dst

    return dst

if __name__ == "__main__":

    joint_line_pos = []
    center_line_pos = []

    #start = time()

    SHAPE = (512, 512)

    WEIGHT_DIR = "weights/log01_dlink34_bone.th"
    RESULT_DIR = "predict_result/bone"

    if not os.path.exists(RESULT_DIR):
        os.makedirs(RESULT_DIR)

    solver = MyFrame(DinkNet34, dice_bce_loss, 2e-4)


    solver.load(WEIGHT_DIR)
    #print("load trained model")

    # for i in file_list:
    raw_img_path = sys.argv[1]

    #raw_img_path = "C:/Users/z00491jc/Desktop/private/CV/IIML/Ibrahim/segmentation/patient00006_study1_positive_image1.jpg"
    #map_run_time = time()
    # src = cv2.imread(raw_imgdir + i, cv2.IMREAD_COLOR)
    src = cv2.imread(raw_img_path, cv2.IMREAD_COLOR)
    src = cv2.resize(src, (512, 512))
    h, w, c = src.shape
    img = np.array(src)

    dst = src.copy()
    dist = []
    temp = np.array(src)
    temp = np.array(temp, np.float32).transpose(2, 0, 1) / 255.0
    dist.append(temp)

    dist = np.array(dist)
    img = torch.Tensor(dist)

    solver.set_input(img)
    _ = solver.forward()

    t_img = solver.test_one_img_to_real(img)

    t_img = t_img[0]
    t_img = t_img.transpose(1, 2, 0)

    t_img_out = t_img
    t_img_out = t_img_out.transpose(2, 0, 1)
    t_img_out = np.uint8(t_img_out.transpose(2, 0, 1))

    iter_img = solver.test_one_mask_to_real(img)
    iter_img = np.uint8(iter_img)

    t_out = Image.fromarray(iter_img, mode='L')

    iter_img = np.uint8(iter_img.reshape(1, 512, 512))

    final_output = Image.new("L", (w, h))

    final_output.paste(t_out, (0, 0))
    RESULT_OUT_DIR = os.path.join(RESULT_DIR, 'predicted_img.jpg')
    final_output.save(RESULT_OUT_DIR)

    # segmentation results - final_output

    filter_list = [1, 2, 3, 3]
    smoothing_iter = 2
    sharpening_iter = 2

    img = cv2.imread(RESULT_OUT_DIR)
    for j in filter_list:
        dst = smoothing(img, smoothing_iter, j)
        dst[dst >= 96] = 255
        dst[dst < 96] = 0
        img = dst
        dst = sharpening(img, sharpening_iter)
        img = dst

    #RESULT_OUT_DIR = os.path.join(RESULT_DIR, 'predicted_img_post.jpg')
    #cv2.imwrite(RESULT_OUT_DIR, dst)

    # post processing resutls - dst

    result_img = dst.copy()

    dst[dst == 255] = True
    dst[dst != True] = False

    starting = 0
    ending = 0
    top_end = 0

    for k in range(dst.shape[0]):
        if dst[k, :].any():
            starting = k
            for kk in range(dst.shape[1]):
                if dst[k, kk].all():
                    top_end = kk
                    break
            break


    for k in range(starting, dst.shape[0]):
        if not dst[k, :].any():
            ending = k
            break
    if ending == 0:
        ending = dst.shape[0] - 1

    left_end = dst.shape[0] - 1
    left_top = 0
    right_end = 0
    right_top = 0

    for k in range(starting, int(starting + (ending - starting) / 4)):
        for kk in range(dst.shape[1]):
            if dst[k, kk].all():
                if kk < left_end:
                    left_end = kk
                    left_top = k
                if kk > right_end:
                    right_end = kk
                    right_top = k

    pos = []
    # joint line points -- pos

    pos.append((top_end, starting))
    if (abs(top_end - left_end) < abs(top_end - right_end)):
        pos.append((right_end, right_top))
    else:
        pos.append((left_end, left_top))

    joint_line_pos.append(pos)

    result_img = cv2.line(result_img, pos[0], pos[1], (255, 0, 0), 3)

    left_end = (dst.shape[0] - 1)
    left_top = 0
    right_end = 0
    right_top = 0

    pos_prime = []
    # central line points -- pos

    ending__ = int(starting - (starting - ending) * 1 / 4)
    for kk in range(dst.shape[1]):
        if dst[ending__ - 1, kk].all():
            if kk < left_end:
                left_end = kk
                left_top = ending__ - 1
            if kk > right_end:
                right_end = kk
                right_top = ending__ - 1

    pos_prime.append((int((left_end + right_end) / 2), ending__))

    left_end = (dst.shape[0] - 1)
    left_top = 0
    right_end = 0
    right_top = 0

    ending__ = int(starting - (starting - ending) * 3 / 4)
    for kk in range(dst.shape[1]):
        if dst[ending__ - 1, kk].all():
            if kk < left_end:
                left_end = kk
                left_top = ending__ - 1
            if kk > right_end:
                right_end = kk
                right_top = ending__ - 1

    pos_prime.append((int((left_end + right_end) / 2), ending__))

    pos_prime_v = np.array([pos_prime[0][0] - pos_prime[1][0], pos_prime[0][1] - pos_prime[1][1]])
    pos_v = np.array([pos[0][0] - pos[1][0], pos[0][1] - pos[1][1]])

    norm_pos_v = np.linalg.norm(pos_v)
    norm_pos_prime_v = np.linalg.norm(pos_prime_v)
    inner_product = pos_v[0] * pos_prime_v[0] + pos_v[1] * pos_prime_v[1]
    cos_v = inner_product / (norm_pos_v * norm_pos_prime_v)

    cos_v = (np.arccos(cos_v) / 3.14) * 180

    center_line_pos.append(pos_prime)

    result_img = cv2.line(result_img, pos_prime[0], pos_prime[1], (0, 255, 0), 3)
    #RESULT_OUT_DIR = os.path.join(RESULT_DIR, 'predicted_img_post_line.jpg')
    #cv2.imwrite(RESULT_OUT_DIR, result_img)

    #print("Sucessfully finished")
    print(pos[0][0], pos[0][1], pos[1][0], pos[1][1], pos_prime[0][0], pos_prime[0][1], pos_prime[1][0], pos_prime[1][1])