﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoFrame.Domain.Model;

namespace PhotoFrame.Domain.UseCase
{
    public class ToggleIsFavorite
    {
        private readonly RepositoryMaster repositoryMaster;

        public ToggleIsFavorite(RepositoryMaster repositoryMaster)
        {
            this.repositoryMaster = repositoryMaster;
        }

        public IEnumerable<Photo> Execute(IEnumerable<Photo> photos)
        {
            foreach(Photo photo in photos)
            {
                if (!photo.IsFavorite)
                {
                    photo.MarkAsFavorite();
                    repositoryMaster.StorePhoto(photo);
                }
            }

            return photos;
        } 
    }
}
